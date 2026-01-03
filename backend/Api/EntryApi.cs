using Mess.Api.Data;
using Mess.Auth;
using Mess.Bank;
using Mess.Data;
using Mess.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mess.Api;

[ApiController]
[Authorize]
[Route("api/v1/groups/{groupId:guid}/entries")]
public class EntryApi : Controller
{
    private ICurrentUserService _currentUser;
    private AppDatabaseContext _db;

    public EntryApi(AppDatabaseContext db, ICurrentUserService currentUserService)
    {
        _db = db;
        _currentUser = currentUserService;
    }
    
    [HttpGet]
    [ProducesResponseType<List<ApiEntry>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ServiceFilter(typeof(GroupAccessFilter))]
    public IActionResult GetApiEntries([FromRoute] Guid groupId, [FromQuery] int offset = 0, [FromQuery] int limit = 20)
    {
        
        Group? group = HttpContext.Items["Group"] as Group;
        if (group == null) return NotFound();
        List<ApiEntry?> entries = _db.Entries.Include(x => x.Participants).ThenInclude(x =>x.Member).Where(x => x.GroupId == groupId).Skip(offset).Take(limit).ToList().ConvertAll(x => ApiEntryFromEntry(x));
        return Ok(entries);
    }
    [HttpGet("{entryId:guid}")]
    [ProducesResponseType<ApiEntry>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ServiceFilter(typeof(GroupAccessFilter))]
    public IActionResult GetApiEntry([FromRoute] Guid groupId, [FromRoute] Guid entryId)
    {
        
        Group? group = HttpContext.Items["Group"] as Group;
        if (group == null) return NotFound();
        Entry? entry = _db.Entries.Include(x => x.Participants).ThenInclude(x =>x.Member).FirstOrDefault(x => x.Id == entryId && x.GroupId == groupId);
        if(entry == null) return NotFound();
        return Ok(ApiEntryFromEntry(entry));
    }
    
    /// <summary>
    /// Creates a new Entry for the group
    /// </summary>
    /// <param name="groupId">Id of the group</param>
    /// <param name="entry">Entry to create</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType<ApiEntry>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ServiceFilter(typeof(GroupAccessFilter))]
    public IActionResult CreateApiEntry([FromRoute] Guid groupId, [FromBody] ApiEntryCreateRequest entry)
    {
        
        Group? group = HttpContext.Items["Group"] as Group;
        if (group == null) return NotFound();
        Entry e = new Entry();
        e.Date = entry.Date;
        e.GroupId = group.Id;
        e.Group = group;
        e.Name = entry.Name;
        if (entry.Participants.Count == 0)
        {
            return BadRequest(new ApiError("Entry must have at least one participant"));
        }
        if(entry.Participants.GroupBy(x => x.MemberId, x => x.MemberId, (guid, guids) => new {Count = guids.Count()}).Any(x => x.Count > 1))
        {
            // ToDo: Add member id to error
            return BadRequest(new ApiError("Cannot have same member in entry multiple times"));
        }
        foreach (ApiEntryCreateRequestParticipant participant in entry.Participants)
        {
            Participant p = new Participant();
            p.ExtraParticipants = participant.ExtraParticipants;
            p.PaidAmount = participant.PaidAmount;
            // Check if member exists
            Member? m = group.Members.FirstOrDefault(x => x.Id == participant.MemberId);
            if (m == null)
            {
                return BadRequest(new ApiError("Member '" + participant.MemberId + "' not found in group '" + group.Name +"'"));
            }
            p.MemberId = participant.MemberId;
            p.Member = m;
            e.Participants.Add(p);
        }

        if (e.GetTotalPrice() == 0)
        {
            return BadRequest(new ApiError("Entry must have a total price greater than 0"));
        }

        Banker.ProcessEntryPayments(e, _db);
        _db.Entries.Add(e);
        _db.SaveChanges();
        return Ok(ApiEntryFromEntry(e));
    }

    private ApiEntry? ApiEntryFromEntry(Entry e)
    {
        Member? m = HttpContext.Items["Member"] as Member;
        if(m == null) return null;
        ApiEntry a = new ApiEntry(e);
        a.Participants.ForEach(x => x.PaidAmountString = PriceFormatter.FormatPrice(x.PaidAmount, e.Group));
        a.MyPersonCount = e.Participants.Sum(x => x.MemberId == m.Id ? x.ExtraParticipants + 1 : 0);
        decimal totalPrice = e.GetTotalPrice();
        a.TotalPrice = PriceFormatter.FormatPrice(totalPrice, e.Group);
        a.TotalPersonCount = a.GetTotalPersonCount();
        decimal ppp = totalPrice / a.TotalPersonCount;
        a.PricePerPerson = PriceFormatter.FormatPrice(ppp, e.Group);
        a.MyPrice = PriceFormatter.FormatPrice(a.MyPersonCount * ppp, e.Group);
        return a;
    }
}
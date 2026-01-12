using Mess.Api.Data;
using Mess.Auth;
using Mess.Bank;
using Mess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mess.Api;

[ApiController]
[Authorize]
[Route("api/v1/groups/{groupId:guid}/balances")]
public class BalanceApi : Controller
{
    private ICurrentUserService _currentUser;
    private AppDatabaseContext _db;

    public BalanceApi(AppDatabaseContext db, ICurrentUserService currentUserService)
    {
        _db = db;
        _currentUser = currentUserService;
    }
    
    [HttpGet]
    [ProducesResponseType<List<Balance>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ServiceFilter(typeof(GroupAccessFilter))]
    public IActionResult GetBalance([FromRoute] Guid groupId)
    {
        Group? group = HttpContext.Items["Group"] as Group;
        if (group == null) return NotFound();
        List<Balance> entries = _db.Balances.Include(x => x.Payee).Include(x => x.Payer).Where(x => x.GroupId == groupId).ToList();
        return Ok(entries);
    }
    
    [HttpPost("recalculate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiError>(StatusCodes.Status500InternalServerError)]
    [ServiceFilter(typeof(GroupAccessFilter))]
    public IActionResult RecalculateBalances([FromRoute] Guid groupId)
    {
        Group? group = HttpContext.Items["Group"] as Group;
        User? currentUser = HttpContext.Items["CurrentUser"] as User;
        if (currentUser == null) return Unauthorized();
        if (group == null) return NotFound();
        if(!group.CanBeEditedBy(currentUser)) return Ok();
        
        // Delete existing balances
        _db.Balances.Where(x => x.GroupId == groupId).ExecuteDelete();
        
        // And recalculate, weeeeeee
        foreach(Entry e in _db.Entries.Include(x => x.Participants).Where(x => x.GroupId == groupId))
        {
            if (!Banker.ProcessEntryPayments(e, _db)) return StatusCode(500, new ApiError("Error processing entry '" + e.Id + "'"));
        }

        // Finally save after recalculating
        _db.SaveChanges();
        return Ok();
    }
}
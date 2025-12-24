using Mess.Auth;
using Mess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mess.Api;

[ApiController]
[Route("api/v1/groups/{groupId:guid}/members")]
public class MemberApi : Controller
{
    
    private ICurrentUserService _currentUser;
    private AppDatabaseContext _db;
    public MemberApi(AppDatabaseContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }


    [HttpGet]
    public IActionResult Get([FromRoute] Guid groupId)
    {
        User? loggedInUser = _currentUser.GetCurrentUser();
        if (loggedInUser == null) return Unauthorized();
        Group? g = _db.Groups.Include(x => x.Members).ThenInclude(x => x.User).FirstOrDefault(x => x.Id == groupId);
        if(g == null) return NotFound();
        if (!g.CanBeAccessedBy(loggedInUser)) return Forbid();
        return Ok(g.Members);
    }
    
    [HttpGet("{memberId:guid}")]
    public IActionResult Get([FromRoute] Guid groupId, [FromRoute] Guid memberId)
    {
        User? loggedInUser = _currentUser.GetCurrentUser();
        if (loggedInUser == null) return Unauthorized();
        Group? g = _db.Groups.Include(x => x.Members).ThenInclude(x => x.User).FirstOrDefault(x => x.Id == groupId);
        if(g == null) return NotFound();
        if (!g.CanBeAccessedBy(loggedInUser)) return Forbid();
        Member? m = g.Members.FirstOrDefault(x => x.Id == memberId);
        if (m == null) return NotFound();
        return Ok(m);
    }

    [HttpPost]
    public IActionResult CreateMember([FromRoute] Guid groupId, [FromBody] Member member)
    {
        User? loggedInUser = _currentUser.GetCurrentUser();
        if (loggedInUser == null) return Unauthorized();
        Group? g = _db.Groups.Include(x => x.Members).FirstOrDefault(x => x.Id == groupId);
        if(g == null) return NotFound();
        if (!g.CanBeEditedBy(loggedInUser)) return Forbid();
        
        // Check if member with id or name already exists
        if (g.Members.Any(x => x.Name == member.Name))
            return BadRequest("A member with this name already exists. Please choose a different one");
        if (member.Id != null)
            return BadRequest("To create a Member the Id must be null. If you want to edit a member make a PUT request to members/{memberId}");
        
        g.Members.Add(member);
        _db.SaveChanges();
        return Ok(member);
    }
    [HttpPut("{memberId:guid}")]
    public IActionResult UpdateMember([FromRoute] Guid groupId, [FromRoute] Guid memberId, [FromBody] Member member)
    {
        User? loggedInUser = _currentUser.GetCurrentUser();
        if (loggedInUser == null) return Unauthorized();
        Group? g = _db.Groups.Include(x => x.Members).FirstOrDefault(x => x.Id == groupId);
        if(g == null) return NotFound();
        if (!g.CanBeEditedBy(loggedInUser)) return Forbid();

        
        // Check if member with id exists
        Member? m = g.Members.FirstOrDefault(x => x.Id == memberId);
        if(m == null) 
            return NotFound("This member does not exist");
        
        // We do not validate the id in the body as it's already specified in the route
        m.Name = member.Name;
        _db.SaveChanges();
        return Ok(m);
    }
    
    [HttpDelete("{memberId:guid}")]
    public IActionResult DeleteMember([FromRoute] Guid groupId, [FromRoute] Guid memberId, [FromBody] Member member)
    {
        User? loggedInUser = _currentUser.GetCurrentUser();
        if (loggedInUser == null) return Unauthorized();
        Group? g = _db.Groups.Include(x => x.Members).FirstOrDefault(x => x.Id == groupId);
        if(g == null) return NotFound();
        if (!g.CanBeEditedBy(loggedInUser)) return Forbid();

        
        // Check if member with id exists
        Member? m = g.Members.FirstOrDefault(x => x.Id == memberId);
        if(m == null) 
            return NotFound("This member does not exist");

        // We just remove them from the group and change the name as the member may still have participated in entries.
        // By doing it like this we don't have to worry about breaking some entry.
        g.Members.Remove(m);
        m.Name = "<deleted member>";
        m.User = null;
        _db.SaveChanges();
        return Ok(m);
    }
}
using Mess.Api.Data;
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


    /// <summary>
    /// Gets a list of all members in the group
    /// </summary>
    /// <param name="groupId">Id of the group</param>
    /// <returns>List of Members</returns>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<List<Member>>(StatusCodes.Status200OK)]
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
    
    /// <summary>
    /// Gets a specific member from a group
    /// </summary>
    /// <param name="groupId">Id of the group</param>
    /// <param name="memberId">Id of the member</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Member>(StatusCodes.Status200OK)]
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

    /// <summary>
    /// Created a new member for the group
    /// </summary>
    /// <param name="groupId">Id of the group to create the member in</param>
    /// <param name="member">The member to create</param>
    /// <returns>The created member</returns>
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Member>(StatusCodes.Status200OK)]
    [HttpPost]
    public IActionResult CreateMember([FromRoute] Guid groupId, [FromBody]  ApiMemberRequest member)
    {
        User? loggedInUser = _currentUser.GetCurrentUser();
        if (loggedInUser == null) return Unauthorized();
        Group? g = _db.Groups.Include(x => x.Members).FirstOrDefault(x => x.Id == groupId);
        if(g == null) return NotFound();
        if (!g.CanBeEditedBy(loggedInUser)) return Forbid();
        
        // Check if member with id or name already exists
        if (g.Members.Any(x => x.Name == member.Name))
            return BadRequest("A member with this name already exists. Please choose a different one");
        
        g.Members.Add(new Member(member));
        _db.SaveChanges();
        return Ok(member);
    }
    
    /// <summary>
    /// Updates a member
    /// </summary>
    /// <param name="groupId">Id of the group</param>
    /// <param name="memberId">Id of the member to update</param>
    /// <param name="member">New member information</param>
    /// <returns>The updated member</returns>
    [HttpPut("{memberId:guid}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Member>(StatusCodes.Status200OK)]
    public IActionResult UpdateMember([FromRoute] Guid groupId, [FromRoute] Guid memberId, [FromBody] ApiMemberRequest member)
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
    
    /// <summary>
    /// Removes a member from the group. It'll still remain in the database so referencing entries won't break but the name will be removed from the entry
    /// </summary>
    /// <param name="groupId">Id of the group</param>
    /// <param name="memberId">Id of thhe Member to remove</param>
    /// <returns></returns>
    [HttpDelete("{memberId:guid}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Member>(StatusCodes.Status200OK)]
    public IActionResult DeleteMember([FromRoute] Guid groupId, [FromRoute] Guid memberId)
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
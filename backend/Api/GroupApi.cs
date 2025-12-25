using System.Threading.Tasks.Dataflow;
using Mess.Api.Data;
using Mess.Auth;
using Mess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mess.Api;

[ApiController]
[Route("api/v1/groups")]
public class GroupApi : Controller
{
    private ICurrentUserService _currentUser;
    private AppDatabaseContext _db;
    public GroupApi(AppDatabaseContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Gets all groups the currently logged in user is part of as a member
    /// </summary>
    /// <returns>Groups of the logged in user</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<Group>(StatusCodes.Status200OK)]
    public IActionResult GetGroups()
    {
        User? loggedInUser = _currentUser.GetCurrentUser();
        if (loggedInUser == null) return Unauthorized();
        return Ok(_db.Groups.Where(x => x.Members.Any(x => x.UserId == loggedInUser.Id)).ToList());
    }

    /// <summary>
    /// Created a new group with the logged in user as member
    /// </summary>
    /// <returns>The created group</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<Group>(StatusCodes.Status200OK)]
    public IActionResult CreateGroup([FromBody] ApiGroupRequest group)
    {
        User? loggedInUser = _currentUser.GetCurrentUser();
        if (loggedInUser == null) return Unauthorized();
        Group newGroup = new Group();
        Member firstMember = new Member(loggedInUser);
        newGroup.Members.Add(firstMember);
        newGroup.Name = group.Name;
        newGroup.CurrencyPostfix = group.CurrencyPostfix;

        _db.Groups.Add(newGroup);
        _db.SaveChanges();
        return Ok(newGroup);
    }

    /// <summary>
    /// Updates a groups info like name and currency
    /// </summary>
    /// <param name="groupId">Id of the group to edit</param>
    /// <param name="group">Updated group info</param>
    /// <returns>The updated group</returns>
    [HttpPut("{groupId:guid}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Group>(StatusCodes.Status200OK)]
    public IActionResult UpdateGroup([FromRoute] Guid groupId, [FromBody] ApiGroupRequest group)
    {
        
        User? loggedInUser = _currentUser.GetCurrentUser();
        if (loggedInUser == null) return Unauthorized();
        Group? g = _db.Groups.FirstOrDefault(x => x.Id == groupId);
        if(g == null) return NotFound();
        if (!g.CanBeEditedBy(loggedInUser)) return Forbid();
        
        g.Name = group.Name;
        g.CurrencyPostfix = group.CurrencyPostfix;
        _db.SaveChanges();
        return Ok(g);
    }
    
    /// <summary>
    /// Gets a specific group
    /// </summary>
    /// <param name="groupId">Id of the group to get</param>
    /// <returns>Full group info</returns>
    [HttpGet("{groupId:guid}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<Group>(StatusCodes.Status200OK)]
    public IActionResult GetGroup([FromRoute] Guid groupId)
    {
        
        User? loggedInUser = _currentUser.GetCurrentUser();
        if (loggedInUser == null) return Unauthorized();
        Group? g = _db.Groups.Include(x => x.Members).ThenInclude(x => x.User).FirstOrDefault(x => x.Id == groupId);
        if(g == null) return NotFound();
        if (!g.CanBeAccessedBy(loggedInUser)) return Forbid();

        _db.SaveChanges();
        return Ok(g);
    }
}
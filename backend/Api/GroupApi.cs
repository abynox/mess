using System.Threading.Tasks.Dataflow;
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

    [HttpGet]
    public IActionResult GetGroups()
    {
        User? loggedInUser = _currentUser.GetCurrentUser();
        if (loggedInUser == null) return Unauthorized();
        return Ok(_db.Groups.Where(x => x.Members.Any(x => x.UserId == loggedInUser.Id)).ToList());
    }

    [HttpPost]
    public IActionResult CreateGroup([FromBody] Group group)
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

    [HttpPut("{groupId:guid}")]
    public IActionResult UpdateGroup([FromRoute] Guid groupId, [FromBody] Group group)
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
    
    [HttpGet("{groupId:guid}")]
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
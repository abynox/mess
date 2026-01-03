using Mess.Api.Data;
using Mess.Auth;
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
    public IActionResult GetApiEntries([FromRoute] Guid groupId)
    {
        Group? group = HttpContext.Items["Group"] as Group;
        if (group == null) return NotFound();
        List<Balance> entries = _db.Balances.Include(x => x.Payee).Include(x => x.Payer).Where(x => x.GroupId == groupId).ToList();
        return Ok(entries);
    }
}
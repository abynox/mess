using Mess.Auth;
using Mess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mess.Api;

[Authorize]
[ApiController]
[Route("/api/v1/me")]
public class UserApi : Controller
{
    ICurrentUserService _currentUserService;
    public UserApi(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }
    
    [HttpGet]
    public IActionResult Get()
    {
        User? u = _currentUserService.GetCurrentUser();
        if (u == null) return Forbid();
        return Ok(u);
    }
}
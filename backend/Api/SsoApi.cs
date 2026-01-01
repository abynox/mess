using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Mess.Auth;
using Mess.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Mess.Api;

[ApiController]
[Route("/api/v1/sso")]
public class SsoApi : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly AppDatabaseContext _db;
    private readonly ICurrentUserService _currentUser;
    
    public SsoApi(IWebHostEnvironment env, AppDatabaseContext db, ICurrentUserService currentUserService)
    {
        _env = env;
        _db = db;
        _currentUser = currentUserService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new List<object>{HttpContext.Request.Headers["X-Forwarded-Proto"], HttpContext.Request.Headers["X-Forwarded-For"], HttpContext.Request.Headers["X-Forwarded-Host"], HttpContext.Request.Scheme});
    }

    [HttpGet("signout")]
    public async Task<IActionResult> Signout()
    {
        await HttpContext.SignOutAsync();
        return Redirect("/");
    }

    [HttpGet("startlogin")]
    public IActionResult CheckLoginType()
    {
        return Redirect("/api/v1/sso/start");
        // Left in from MFS
        if (Config.Instance.UseOAuth) return Redirect("/api/v1/sso/start");
        return Redirect("/password");
    }
    
    private string GenerateJSONWebToken(string sub)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.Instance.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, sub),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(Config.Instance.JwtIssuer,
            Config.Instance.JwtIssuer,
            claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpGet("redirectlogin")]
    [Authorize(AuthenticationSchemes = "oidc")]
    public IActionResult RedirectLogin([FromQuery] string redirectUrl)
    {
        User? currentUser = _currentUser.GetCurrentUser();
        if (currentUser == null) return Unauthorized();

        return Redirect(redirectUrl + "?jwt=" + GenerateJSONWebToken(currentUser.OidcId));
    }

    [HttpGet("start")]
    [Authorize(AuthenticationSchemes = "oidc")]
    public async Task<IActionResult> StartLogin([FromQuery] string redirectUrl = "/")
    {
        return Redirect(redirectUrl);
    }
}
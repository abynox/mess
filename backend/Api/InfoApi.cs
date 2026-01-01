using Mess.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Mess.Api;

[ApiController]
[Route("/api/v1/info")]
public class InfoApi : Controller
{
    public IActionResult Get()
    {
        return Ok(new MessInfo());
    }
}
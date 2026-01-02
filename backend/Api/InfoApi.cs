using System.Net.Mime;
using Mess.Api.Data;
using Mess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Mess.Api;

[ApiController]
[Route("/api/v1/info")]
public class InfoApi : Controller
{
    [HttpGet]
    [ProducesResponseType<MessInfo>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    public IActionResult Get()
    {
        return Ok(new MessInfo());
    }
}
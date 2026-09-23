using System.Security.Claims;
using EsportsAPI.DTOs;
using EsportsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsportsAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto)
    {
        var result = await _service.Register(dto);

        if (result)
            return Ok();
        else
            return Conflict("Username is already taken");
    }

    [HttpPost("login")]
    public async Task<ActionResult<string?>> Login(LoginDto dto)
    {
        var jwt = await _service.Login(dto);

        if (jwt == null)
            return Unauthorized("Incorrect username or password");

        return Ok(jwt);
    }

    [HttpPost("promote")]
    [Authorize]
    public async Task<ActionResult> Promote(PromoteDto dto)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _service.Promote(userId, dto.Key);

        if (!result)
            return Conflict("Wrong key to promote to organizer");

        return Ok("Promoted to Organizer. Log in again to access Organizer actions");
    }
}
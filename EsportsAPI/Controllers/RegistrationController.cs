using EsportsAPI.DTOs;
using EsportsAPI.Entities;
using EsportsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsportsAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationService _service;

    public RegistrationController(IRegistrationService service)
    {
        _service = service;
    }

    [HttpPost("{tournamentId}/{teamId}")]
    [Authorize]
    public async Task<ActionResult<RegistrationDto>> RegisterTeam(int tournamentId, int teamId)
    {
        var result = await _service.RegisterTeam(tournamentId, teamId);

        return result.Status switch
        {
            RegistrationResultStatus.Success => Ok(result.Dto),
            RegistrationResultStatus.TournamentNotFound => NotFound("Tournament Not Found"),
            RegistrationResultStatus.TeamNotFound => NotFound("Team Not Found"),
            RegistrationResultStatus.AlreadyRegistered => Conflict("Team Already Registered in Tournament"),
            RegistrationResultStatus.TournamentFull => Conflict("Tournament is full"),
            RegistrationResultStatus.TournamentNotOpen => Conflict("Tournament closed for registration"),
            _ => StatusCode(500, "Unexpected error")  
        };
    }

    [HttpGet("{tournamentId}")]
    public async Task<ActionResult<List<RegistrationDto>>> GetRegisteredTeams(int tournamentId)
    {
        var result = await _service.GetRegisteredTeams(tournamentId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
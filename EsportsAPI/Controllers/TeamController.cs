using EsportsAPI.DTOs;
using EsportsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsportsAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class TeamController : ControllerBase
{
    private readonly ITeamService _service;

    public TeamController(ITeamService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<TeamDto>>> GetAllTeams()
    {
        var teamDtos = await _service.GetAll();

        return Ok(teamDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDto>> GetTeam(int id)
    {
        var teamDto = await _service.GetById(id);

        if (teamDto == null) return NotFound();

        return Ok(teamDto);
    }

    [HttpPost]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<TeamDto>> CreateTeam(CreateTeamDto incomingTeam)
    {
        var teamToReturn = await _service.CreateTeam(incomingTeam);

        return CreatedAtAction(nameof(GetTeam), new {id = teamToReturn.Id}, teamToReturn);
    }
}
using EsportsAPI.Data;
using EsportsAPI.DTOs;
using EsportsAPI.Entities;
using EsportsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EsportsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TournamentController : ControllerBase
{
    private readonly ITournamentService _service;

    public TournamentController(ITournamentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<TournamentDto>>> GetTournaments()
    {
        var tournamentDtos = await _service.GetAll();

        return Ok(tournamentDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDto>> GetTournament(int id)
    {
        var tournamentDto = await _service.GetById(id);

        if (tournamentDto == null) return NotFound();

        return Ok(tournamentDto);
    }

    [HttpPost]
    public async Task<ActionResult<TournamentDto>> CreateTournament(CreateTournamentDto incomingTournament)
    {
        var tournamentToReturn = await _service.Create(incomingTournament);

        return CreatedAtAction(nameof(GetTournament), new { id = tournamentToReturn.Id }, tournamentToReturn);
    }
}
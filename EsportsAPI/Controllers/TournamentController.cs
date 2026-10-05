using EsportsAPI.Data;
using EsportsAPI.DTOs;
using EsportsAPI.Entities;
using EsportsAPI.Services;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<TournamentDto>> CreateTournament(CreateTournamentDto incomingTournament)
    {
        var tournamentToReturn = await _service.Create(incomingTournament);

        return CreatedAtAction(nameof(GetTournament), new { id = tournamentToReturn.Id }, tournamentToReturn);
    }

    [HttpPost("{id}/lock")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<TransitionResultStatus>> Lock(int id)
    {
        var transitionStatus = await _service.Lock(id);

        return transitionStatus switch
        {
            TransitionResultStatus.NotAllowed => Conflict("Cannot lock this tournament"),
            TransitionResultStatus.TournamentNotFound => NotFound("Tournament Not Found"),
            TransitionResultStatus.EmptyTournament => Conflict("Tournament is empty"),
            TransitionResultStatus.OddTeamCount => Conflict("Cannot start tournament with odd number of teams"),
            TransitionResultStatus.Success => Ok(transitionStatus),
            _ => StatusCode(500, "Unexpected Error")
        };
    }

    [HttpPost("{id}/start")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<TransitionResultStatus>> Start(int id)
    {
        var transitionStatus = await _service.Start(id);

        return transitionStatus switch
        {
            TransitionResultStatus.NotAllowed => Conflict("Cannot start this tournament"),
            TransitionResultStatus.TournamentNotFound => NotFound("Tournament Not Found"),
            TransitionResultStatus.Success => Ok(transitionStatus),
            _ => StatusCode(500, "Unexpected Error")
        };
    }

    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Organizer")]
    public async Task<ActionResult<TransitionResultStatus>> Complete(int id)
    {
        var transitionStatus = await _service.Complete(id);

        return transitionStatus switch
        {
            TransitionResultStatus.NotAllowed => Conflict("Cannot complete this tournament"),
            TransitionResultStatus.TournamentNotFound => NotFound("Tournament Not Found"),
            TransitionResultStatus.Success => Ok(transitionStatus),
            _ => StatusCode(500, "Unexpected Error")
        };
    }
}
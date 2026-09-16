using EsportsAPI.Data;
using EsportsAPI.DTOs;
using EsportsAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EsportsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TournamentController : ControllerBase
{
    private readonly EsportsDbContext _context;

    public TournamentController(EsportsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<TournamentDto>>> GetTournaments()
    {
        var tournaments = await _context.Tournaments.ToListAsync();

        var tournamentDtos = tournaments.Select(tournament => new TournamentDto
        {
            Id = tournament.Id,
            Name = tournament.Name,
            GameTitle = tournament.GameTitle,
            Status = tournament.Status,
            MaxTeams = tournament.MaxTeams,
            StartDate = tournament.StartDate
        }).ToList();

        return Ok(tournamentDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDto>> GetTournament(int id)
    {
        var tournament = await _context.Tournaments.FindAsync(id);

        if (tournament == null)
        {
            return NotFound();
        }

        var tournamentDto = new TournamentDto
        {
            Id = tournament.Id,
            Name = tournament.Name,
            GameTitle = tournament.GameTitle,
            Status = tournament.Status,
            MaxTeams = tournament.MaxTeams,
            StartDate = tournament.StartDate
        };

        return Ok(tournamentDto);
    }

    [HttpPost]
    public async Task<ActionResult<TournamentDto>> CreateTournament(CreateTournamentDto incomingTournament)
    {
        var tournament = new Tournament
        {
            Name = incomingTournament.Name,
            GameTitle = incomingTournament.GameTitle,
            MaxTeams = incomingTournament.MaxTeams,
            StartDate = incomingTournament.StartDate.Value
        };

        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();

        var tournamentToReturn = new TournamentDto
        {
            Id = tournament.Id,
            Name = tournament.Name,
            GameTitle = tournament.GameTitle,
            Status = tournament.Status,
            MaxTeams = tournament.MaxTeams,
            StartDate = tournament.StartDate
        };

        return CreatedAtAction(nameof(GetTournament), new { id = tournament.Id }, tournamentToReturn);
    }
}
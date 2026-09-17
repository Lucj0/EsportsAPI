using EsportsAPI.Data;
using EsportsAPI.DTOs;
using EsportsAPI.Entities;
using EsportsAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace EsportsAPI.Services;

public class TournamentService : ITournamentService
{
    private readonly EsportsDbContext _context;

    public TournamentService(EsportsDbContext context)
    {
        _context = context;
    }

    public async Task<List<TournamentDto>> GetAll()
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

        return tournamentDtos;
    }

    public async Task<TournamentDto?> GetById(int id)
    {
        var tournament = await _context.Tournaments.FindAsync(id);

        if (tournament == null)
            return null;

        var tournamentDto = new TournamentDto
        {
            Id = tournament.Id,
            Name = tournament.Name,
            GameTitle = tournament.GameTitle,
            Status = tournament.Status,
            MaxTeams = tournament.MaxTeams,
            StartDate = tournament.StartDate
        };

        return tournamentDto;
    }

    public async Task<TournamentDto> Create(CreateTournamentDto incomingTournament)
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

        return tournamentToReturn;
    }


}
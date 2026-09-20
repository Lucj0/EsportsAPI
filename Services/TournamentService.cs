using EsportsAPI.Data;
using EsportsAPI.DTOs;
using EsportsAPI.Entities;
using EsportsAPI.Services;
using Microsoft.AspNetCore.Mvc;
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

    public async Task<TransitionResultStatus> Lock(int id)
    {
        var tournament = await _context.Tournaments.FindAsync(id);

        if (tournament == null)
            return TransitionResultStatus.TournamentNotFound;

        if (tournament.Status != TournamentStatus.Registration)
            return TransitionResultStatus.NotAllowed;

        var registrations = await _context.Registrations
            .Where(r => r.TournamentId == tournament.Id)
            .ToListAsync();

        if (registrations.Count == 0)
            return TransitionResultStatus.EmptyTournament;

        if (registrations.Count % 2 != 0)
            return TransitionResultStatus.OddTeamCount;

        tournament.Status = TournamentStatus.Locked;

        var shuffled = registrations.OrderBy(r => Random.Shared.Next()).ToList();

        for (int i = 0; i < shuffled.Count; i++)
        {
            shuffled[i].Seed = i + 1;
        }

        await _context.SaveChangesAsync();

        return TransitionResultStatus.Success;
    }

    public async Task<TransitionResultStatus> Start(int id)
    {
        return await Transition(id, TournamentStatus.Locked, TournamentStatus.InProgress);
    }

    public async Task<TransitionResultStatus> Complete(int id)
    {
        return await Transition(id, TournamentStatus.InProgress, TournamentStatus.Complete);
    }

    private async Task<TransitionResultStatus> Transition(int id, TournamentStatus required, TournamentStatus desired)
    {
        var tournament = await _context.Tournaments.FindAsync(id);

        if (tournament == null)
            return TransitionResultStatus.TournamentNotFound;

        if (tournament.Status != required)
            return TransitionResultStatus.NotAllowed;

        tournament.Status = desired;

        await _context.SaveChangesAsync();

        return TransitionResultStatus.Success;
    }
}
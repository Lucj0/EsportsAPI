

using EsportsAPI.Data;
using EsportsAPI.DTOs;
using EsportsAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace EsportsAPI.Services;

public class RegistrationService : IRegistrationService
{
    private readonly EsportsDbContext _context;

    public RegistrationService(EsportsDbContext context)
    {
        _context = context;
    }

    public async Task<RegistrationResult> RegisterTeam(int tournamentId, int teamId)
    {
        var tournament = await _context.Tournaments.FindAsync(tournamentId);
        var team = await _context.Teams.FindAsync(teamId);
        var registrationResult = new RegistrationResult();

        if (tournament == null)
        {
            registrationResult.Status = RegistrationResultStatus.TournamentNotFound;
            return registrationResult;
        }

        if (team == null)
        {
            registrationResult.Status = RegistrationResultStatus.TeamNotFound;
            return registrationResult;
        }

        if (tournament.Status != TournamentStatus.Registration)
        {
            registrationResult.Status = RegistrationResultStatus.TournamentNotOpen;
            return registrationResult;
        }

        bool already = await _context.Registrations.AnyAsync(r => r.TournamentId == tournamentId && r.TeamId == teamId);

        if (already)
        {
            registrationResult.Status = RegistrationResultStatus.AlreadyRegistered;
            return registrationResult;
        }

        int count = await _context.Registrations.CountAsync(r => r.TournamentId == tournamentId);

        if (count >= tournament.MaxTeams)
        {
            registrationResult.Status = RegistrationResultStatus.TournamentFull;
            return registrationResult;
        }

        var registration = new Registration
        {
            TournamentId = tournamentId,
            Tournament = tournament,
            TeamId = teamId,
            Team = team,
            RegisteredAt = DateTime.UtcNow
        };

        _context.Registrations.Add(registration);
        await _context.SaveChangesAsync();

        var registrationDto = new RegistrationDto
        {
            Id = registration.Id,
            TournamentId = registration.TournamentId,
            TournamentName = registration.Tournament.Name,
            TeamId = registration.TeamId,
            TeamName = registration.Team.Name,
            RegisteredAt = registration.RegisteredAt
        };

        registrationResult.Status = RegistrationResultStatus.Success;
        registrationResult.Dto = registrationDto;

        return registrationResult;
    }

    public async Task<List<RegistrationDto>?> GetRegisteredTeams(int tournamentId)
    {
        var tournament = await _context.Tournaments.FindAsync(tournamentId);

        if (tournament == null)
            return null;

        var registrations = await _context.Registrations
            .Where(r => r.TournamentId == tournamentId)
            .Include(r => r.Tournament)
            .Include(r => r.Team)
            .ToListAsync();

        var registrationDtos = registrations.Select(registration => new RegistrationDto
        {
            Id = registration.Id,
            TournamentId = registration.TournamentId,
            TournamentName = registration.Tournament.Name,
            TeamId = registration.TeamId,
            TeamName = registration.Team.Name,
            RegisteredAt = registration.RegisteredAt,
            Seed = registration.Seed
        }).ToList();

        return registrationDtos;
    }
}
using EsportsAPI.DTOs;

namespace EsportsAPI.Services;

public interface IRegistrationService
{
    Task<RegistrationResult> RegisterTeam(int tournamentId, int teamId);

    Task<List<RegistrationDto>?> GetRegisteredTeams(int tournamentId);
}
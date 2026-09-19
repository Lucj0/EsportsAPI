namespace EsportsAPI.Services;

public interface IRegistrationService
{
    Task<RegistrationResult> RegisterTeam(int tournamentId, int teamId);
}
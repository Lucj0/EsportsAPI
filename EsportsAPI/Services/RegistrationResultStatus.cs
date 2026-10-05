namespace EsportsAPI.Services;

public enum RegistrationResultStatus
{
    Success,
    TournamentNotFound,
    TeamNotFound,
    TournamentNotOpen,
    AlreadyRegistered,
    TournamentFull
}
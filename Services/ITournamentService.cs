using EsportsAPI.DTOs;

namespace EsportsAPI.Services;

public interface ITournamentService
{
    Task<TournamentDto?> GetById(int id);

    Task<List<TournamentDto>> GetAll();

    Task<TournamentDto> Create(CreateTournamentDto incomingTournament);
}
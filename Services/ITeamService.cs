using EsportsAPI.DTOs;

namespace EsportsAPI.Services;

public interface ITeamService
{
    Task<List<TeamDto>> GetAll();

    Task<TeamDto?> GetById(int id);

    Task<TeamDto> CreateTeam(CreateTeamDto incomingTeam);
}
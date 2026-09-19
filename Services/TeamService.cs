using EsportsAPI.Data;
using EsportsAPI.DTOs;
using EsportsAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace EsportsAPI.Services;

public class TeamService : ITeamService
{
    private readonly EsportsDbContext _context;

    public TeamService(EsportsDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeamDto>> GetAll()
    {
        var teams = await _context.Teams.ToListAsync();

        var teamDtos = teams.Select(team => new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Tag = team.Tag
        }).ToList();

        return teamDtos;
    }

    public async Task<TeamDto?> GetById(int id)
    {
        var team = await _context.Teams.FindAsync(id);

        if (team == null)
            return null;

        var teamDto = new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Tag = team.Tag
        };

        return teamDto;
    }

    public async Task<TeamDto> CreateTeam(CreateTeamDto incomingTeam)
    {
        var team = new Team
        {
            Name = incomingTeam.Name,
            Tag = incomingTeam.Tag
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        var teamToReturn = new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Tag = team.Tag
        };

        return teamToReturn;
    }
}
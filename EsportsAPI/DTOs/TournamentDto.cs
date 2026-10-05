using EsportsAPI.Entities;

namespace EsportsAPI.DTOs;

public class TournamentDto
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string GameTitle { get ; set; }

    public TournamentStatus Status { get; set; }

    public int MaxTeams { get; set; }

    public DateTime StartDate { get; set; }
}
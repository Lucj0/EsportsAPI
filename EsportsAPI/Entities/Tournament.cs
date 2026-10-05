namespace EsportsAPI.Entities;

public class Tournament
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string GameTitle { get; set; }

    public TournamentStatus Status { get; set; }

    public int MaxTeams { get; set; }

    public DateTime StartDate { get; set; }
}
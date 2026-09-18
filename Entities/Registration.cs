namespace EsportsAPI.Entities;

public class Registration
{
    public int Id { get; set; }

    public int TournamentId { get ; set; }
    public Tournament Tournament { get; set; } = null!;

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public DateTime RegisteredAt { get; set; }

    public int? Seed { get; set; }
}
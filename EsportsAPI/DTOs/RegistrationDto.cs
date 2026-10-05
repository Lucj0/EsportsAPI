namespace EsportsAPI.DTOs;

public class RegistrationDto
{
    public int Id { get; set; }

    public int TournamentId { get; set; }
    public required string TournamentName { get; set; }
    
    public int TeamId { get; set; }
    public required string TeamName { get; set; }

    public DateTime RegisteredAt { get; set; }

    public int? Seed { get; set; }
}
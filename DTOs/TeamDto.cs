namespace EsportsAPI.DTOs;

public class TeamDto
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Tag { get ; set; }
}
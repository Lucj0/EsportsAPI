namespace EsportsAPI.Entities;

public class Team
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Tag { get ; set; }
}
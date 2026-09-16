using System.ComponentModel.DataAnnotations;
using EsportsAPI.Entities;

namespace EsportsAPI.DTOs;

public class CreateTournamentDto
{
    [Required]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Tournament name must be between 3 and 30 characters.")]
    public required string Name { get; set; }

    [Required]
    [StringLength(100)]
    public required string GameTitle { get; set; }

    [Required]
    [Range(2, 48)]
    public int MaxTeams { get; set; }

    [Required]
    public DateTime? StartDate { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace EsportsAPI.DTOs;

public class RegisterDto
{
    [Required]
    [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 30 characters")]
    public required string Username { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 30 characters")]
    public required string Password { get; set; }
}
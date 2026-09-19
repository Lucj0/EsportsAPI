using EsportsAPI.DTOs;

namespace EsportsAPI.Services;

public class RegistrationResult
{
    public RegistrationResultStatus Status { get; set; }

    public RegistrationDto? Dto { get; set; }
}
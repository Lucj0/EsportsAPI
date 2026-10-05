

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EsportsAPI.Data;
using EsportsAPI.DTOs;
using EsportsAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace EsportsAPI.Services;

public class AuthService : IAuthService
{
    private readonly EsportsDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(EsportsDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<bool> Register(RegisterDto dto)
    {
        var already = await _context.Users.AnyAsync(u => u.Username == dto.Username);

        if (already)
            return false;

        var hashPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = hashPassword,
            Role = UserRole.Participant
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<string?> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (user == null)
            return null;

        var result = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!result)
            return null;
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiryMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> Promote(int userId, string providedKey)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return false;

        if (providedKey != _config["Auth:OrganizerKey"])
            return false;

        user.Role = UserRole.Organizer;
        await _context.SaveChangesAsync();

        return true;
    }
}
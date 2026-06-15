using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RevenueRecognitionSystem.Core.Abstractions;
using RevenueRecognitionSystem.Core.DTOs.AuthenticationDTOs;
using RevenueRecognitionSystem.Data.Context;
using RevenueRecognitionSystem.Data.Entities.Authentication;

namespace RevenueRecognitionSystem.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<TokenResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Login == request.Login);

        if (user is null || !IsPasswordValid(user, request.Password))
        {
            return null;
        }

        return new TokenResponse(CreateToken(user));
    }

    private bool IsPasswordValid(User user, string password)
    {
        return user.PasswordHash == password || user.PasswordHash == HashPassword(password);
    }

    private string CreateToken(User user)
    {
        var secret = _configuration["Jwt:Secret"] ??
                     "super-mega-secret-key-12345-development";
        var issuer = _configuration["Jwt:Issuer"] ?? "RevenueRecognitionSystem";
        var audience = _configuration["Jwt:Audience"] ?? "RevenueRecognitionSystem";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}

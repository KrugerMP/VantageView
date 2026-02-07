using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VantageView.Auth.Models;
using VantageView.Data;
using VantageView.Data.Entities;

namespace VantageView.Auth.Domain.Auth.Commands;

/// <summary>
/// Handles authentication commands (e.g. user login and JWT issuance).
/// </summary>
public class AuthCommands : IAuthCommands
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthCommands> _logger;

    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthCommands"/> class.
    /// </summary>
    /// <param name="configuration">Application configuration for JWT settings.</param>
    /// <param name="logger">The logger for auth diagnostics.</param>
    /// <param name="db">The database context for user lookup.</param>
    public AuthCommands(IConfiguration configuration, ILogger<AuthCommands> logger, AppDbContext db)
    {
        _configuration = configuration;
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// Validates credentials and issues a JWT on success.
    /// </summary>
    /// <param name="loginRequest">The login credentials.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A tuple of (success, message or error text, token or empty).</returns>
    public async Task<(bool logonResult, string message, string token)> LogonUserAsync(LoginRequest loginRequest, CancellationToken ct)
    {
        try
        {
            string passwordHash = HashPasswordSha256(loginRequest.Password);
            Users? userFound = await _db.Users
                .FirstOrDefaultAsync(u => u.UserName == loginRequest.Username && u.Password == passwordHash, ct);

            if (userFound is null)
            {
                _logger.LogWarning("Failed login attempt for user {Username}", loginRequest.Username);

                return (false, string.Empty, "Invalid username or password entered");
            }

            string key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
            string issuer = _configuration["Jwt:Issuer"] ?? "VantageView.Auth";
            string audience = _configuration["Jwt:Audience"] ?? "VantageView.API";

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] tokenKey = Encoding.UTF8.GetBytes(key);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, loginRequest.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                ]),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return (true, string.Empty, tokenString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when trying to sign in user: {UserName}", loginRequest.Username);
            return (false, "Invalid username or password entered", string.Empty);
        }
    }

    /// <summary>
    /// Computes the SHA-256 hash of the input as a lowercase hex string.
    /// </summary>
    /// <param name="input">The string to hash (e.g. password).</param>
    /// <returns>The hex-encoded hash, or empty string if input is null or empty.</returns>
    private static string HashPasswordSha256(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }
            
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = SHA256.HashData(bytes);

        // Most common format: lowercase hex without dashes
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
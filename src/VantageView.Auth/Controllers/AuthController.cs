using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VantageView.Auth.Models;

namespace VantageView.Auth.Controllers;

/// <summary>
/// Authentication endpoints for admin login and JWT issuance.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="configuration">Application configuration for JWT settings.</param>
    /// <param name="logger">The logger for auth diagnostics.</param>
    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates an admin user and returns a JWT bearer token.
    /// </summary>
    /// <param name="request">Login credentials (simplified demo: admin/admin).</param>
    /// <returns>200 OK with JWT token on success; 401 Unauthorized on invalid credentials.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // Simplified auth for demo: accept admin/admin
        if (request.Username != "admin" || request.Password != "admin")
        {
            _logger.LogWarning("Failed login attempt for user {Username}", request.Username);
            return Unauthorized();
        }

        string key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
        string issuer = _configuration["Jwt:Issuer"] ?? "VantageView.Auth";
        string audience = _configuration["Jwt:Audience"] ?? "VantageView.API";

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] tokenKey = Encoding.UTF8.GetBytes(key);
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, "Admin")
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string tokenString = tokenHandler.WriteToken(token);

        _logger.LogInformation($"User:[{request.Username}] logged in at {DateTime.UtcNow}");

        return Ok(new LoginResponse(tokenString));
    }
}

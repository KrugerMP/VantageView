namespace VantageView.Auth.Models;

/// <summary>
/// Login credentials for admin authentication.
/// </summary>
/// <param name="Username">The admin username.</param>
/// <param name="Password">The admin password.</param>
public record LoginRequest(string Username, string Password);

/// <summary>
/// Response containing the JWT token after successful login.
/// </summary>
/// <param name="Token">The JWT bearer token.</param>
public record LoginResponse(string Token);

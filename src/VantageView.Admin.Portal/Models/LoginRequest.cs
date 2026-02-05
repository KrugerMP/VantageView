using System.ComponentModel.DataAnnotations;

namespace VantageView.Admin.Portal.Models;

/// <summary>
/// Login credentials for admin authentication.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Gets or sets the admin username.
    /// </summary>
    [Required]
    public string Username { get; set; } = "";

    /// <summary>
    /// Gets or sets the admin password.
    /// </summary>
    [Required]
    public string Password { get; set; } = "";
}

/// <summary>
/// Response containing the JWT token after successful login.
/// </summary>
/// <param name="Token">The JWT bearer token.</param>
public record LoginResponse(string Token);

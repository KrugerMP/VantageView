using VantageView.Auth.Models;

namespace VantageView.Auth.Domain.Auth.Commands;

/// <summary>
/// Defines authentication commands (e.g. login).
/// </summary>
public interface IAuthCommands
{
    /// <summary>
    /// Validates credentials and returns a JWT on success.
    /// </summary>
    /// <param name="loginRequest">The login credentials.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A tuple of (success, message or error text, token or empty).</returns>
    Task<(bool logonResult, string message, string token)> LogonUserAsync(LoginRequest loginRequest, CancellationToken ct);
}
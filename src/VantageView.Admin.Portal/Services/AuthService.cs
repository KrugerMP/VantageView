namespace VantageView.Admin.Portal.Services;

/// <summary>
/// Scoped service that holds the current user's JWT token for API requests.
/// </summary>
public class AuthService
{
    /// <summary>
    /// Gets the current JWT bearer token, or <c>null</c> if not authenticated.
    /// </summary>
    public string? Token { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

    /// <summary>
    /// Stores the JWT token after successful login.
    /// </summary>
    /// <param name="token">The JWT bearer token.</param>
    public void SetToken(string token) => Token = token;

    /// <summary>
    /// Clears the stored token on logout.
    /// </summary>
    public void ClearToken() => Token = null;
}

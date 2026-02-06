using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace VantageView.Admin.Portal.Services;

/// <summary>
/// Scoped service that provides auth state and JWT from the current cookie (HttpContext).
/// </summary>
public class AuthService
{
    private const string AccessTokenClaimType = "access_token";
    private const string UsernameClaimType = "username";

    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">Accessor for the current HTTP context.</param>
    public AuthService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the current JWT bearer token from the signed-in user's claims, or <c>null</c> if not authenticated.
    /// </summary>
    public string? Token =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(AccessTokenClaimType);

    /// <summary>
    /// Gets the current username from the signed-in user's claims, or <c>null</c> if not authenticated.
    /// </summary>
    public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(UsernameClaimType);

    /// <summary>
    /// Gets a value indicating whether the user is authenticated (signed in via cookie).
    /// </summary>
    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}

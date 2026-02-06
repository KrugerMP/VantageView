using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using VantageView.Admin.Portal.Models;
using VantageView.Admin.Portal.Services;

namespace VantageView.Admin.Portal.Components.Pages;

/// <summary>
/// Admin login page for authenticating with the Auth service.
/// </summary>
public partial class Login
{
    /// <summary>
    /// Gets or sets the HTTP client factory for Auth API requests.
    /// </summary>
    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = null!;

    /// <summary>
    /// Gets or sets the authentication service for token storage.
    /// </summary>
    [Inject]
    private AuthService AuthService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation manager for redirects after login.
    /// </summary>
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    /// <summary>
    /// Gets or sets the HTTP context for authentication.
    /// </summary>
    [Inject]
    private protected IHttpContextAccessor HttpContextAccessor { get; set; } = null!;

    /// <summary>
    /// Gets or sets the logger for login operations.
    /// </summary>
    [Inject]
    private ILogger<Login> Logger { get; set; } = null!;

    /// <summary>
    /// The login form model (username and password).
    /// </summary>
    private LoginRequest request = new();

    /// <summary>
    /// Whether the login request is in progress.
    /// </summary>
    private bool loading;

    /// <summary>
    /// Error message to display if login fails.
    /// </summary>
    private string? error;

    /// <summary>
    /// Handles form submission to authenticate and obtain a JWT token.
    /// </summary>
    private async Task HandleLoginAsync()
    {
        loading = true;
        error = null;
        try
        {
            if (HttpContextAccessor is null)
            {
                throw new Exception("Cannot continue if no httpcontext is provided");
            }

            HttpClient client = HttpClientFactory.CreateClient("Auth");
            HttpResponseMessage response = await client.PostAsJsonAsync("api/auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                LoginResponse? result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result is not null)
                {
                    List<Claim> claims =
                    [
                        new Claim("username", request.Username),
                        // add more claims you want available in HttpContext.User
                    ];

                    ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new(identity);

                    HttpContext? httpContext = HttpContextAccessor.HttpContext;

                    if (httpContext is null)
                    {
                        throw new Exception("Cannot continue if no httpcontext is provided");
                    }

                    await httpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = false,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                        });

                    Logger.LogInformation("Login successful. Redirecting to articles page.");
                    Navigation.NavigateTo("/articles", forceLoad: true);
                }
            }
            else
            {
                Logger.LogWarning("Login failed. Invalid username or password.");
                error = "Invalid username or password.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Login failed. Ensure the Auth service is running.");
            error = "Login failed. Ensure the Auth service is running.";
        }
        finally
        {
            loading = false;
        }
    }
}

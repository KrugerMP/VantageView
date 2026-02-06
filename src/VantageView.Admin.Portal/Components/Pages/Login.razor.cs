using System.Net.Http.Json;
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
    private async Task HandleLogin()
    {
        loading = true;
        error = null;
        try
        {
            HttpClient client = HttpClientFactory.CreateClient("Auth");
            HttpResponseMessage response = await client.PostAsJsonAsync("api/auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                LoginResponse? result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result is not null)
                {
                    AuthService.SetToken(result.Token);
                    Navigation.NavigateTo("/articles", forceLoad: true);
                }
            }
            else
            {
                error = "Invalid username or password.";
            }
        }
        catch
        {
            error = "Login failed. Ensure the Auth service is running.";
        }
        finally
        {
            loading = false;
        }
    }
}

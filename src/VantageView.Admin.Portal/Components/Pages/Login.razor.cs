using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using VantageView.Admin.Portal.Models;
using VantageView.Admin.Portal.Services;

namespace VantageView.Admin.Portal.Components.Pages;

public partial class Login
{
    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = null!;

    [Inject]
    private AuthService AuthService { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    private LoginRequest request = new();
    private bool loading;
    private string? error;

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

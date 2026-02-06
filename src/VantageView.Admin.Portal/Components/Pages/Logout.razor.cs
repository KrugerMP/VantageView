using Microsoft.AspNetCore.Components;
using VantageView.Admin.Portal.Services;

namespace VantageView.Admin.Portal.Components.Pages;

/// <summary>
/// Logout page that clears the auth token and redirects to login.
/// </summary>
public partial class Logout
{
    /// <summary>
    /// Gets or sets the authentication service for clearing the token.
    /// </summary>
    [Inject]
    private AuthService AuthService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation manager for redirecting to login.
    /// </summary>
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        AuthService.ClearToken();
        Navigation.NavigateTo("/login", forceLoad: true);
    }
}

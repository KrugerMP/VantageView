using Microsoft.AspNetCore.Components;
using VantageView.Admin.Portal.Services;

namespace VantageView.Admin.Portal.Components.Pages;

public partial class Logout
{
    [Inject]
    private AuthService AuthService { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    protected override void OnInitialized()
    {
        AuthService.ClearToken();
        Navigation.NavigateTo("/login", forceLoad: true);
    }
}

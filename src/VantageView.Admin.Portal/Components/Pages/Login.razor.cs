using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components;

namespace VantageView.Admin.Portal.Components.Pages;

/// <summary>
/// Admin login page for authenticating with the Auth service.
/// Form POSTs to /account/login so cookie sign-in runs in a real HTTP request.
/// </summary>
public partial class Login
{
    [Inject]
    private IAntiforgery Antiforgery { get; set; } = null!;

    [Inject]
    private IHttpContextAccessor HttpContextAccessor { get; set; } = null!;

    /// <summary>
    /// Error message from query string (e.g. /login?error=invalid).
    /// </summary>
    [SupplyParameterFromQuery(Name = "error")]
    public string? Error { get; set; }

    /// <summary>
    /// Antiforgery token for the form POST.
    /// </summary>
    private string? AntiforgeryRequestToken { get; set; }

    protected override void OnInitialized()
    {
        HttpContext? context = HttpContextAccessor.HttpContext;
        if (context is not null)
        {
            AntiforgeryTokenSet tokens = Antiforgery.GetAndStoreTokens(context);
            AntiforgeryRequestToken = tokens.RequestToken;
        }
    }

    private string? ErrorMessage =>
        Error switch
        {
            "invalid" => "Invalid username or password.",
            "failed" => "Login failed. Ensure the Auth service is running.",
            _ => null
        };
}

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Components;

namespace VantageView.Admin.Portal.Components.Pages;

/// <summary>
/// Admin login page for authenticating with the Auth service.
/// Form POSTs to /account/login so cookie sign-in runs in a real HTTP request.
/// </summary>
public partial class Login
{
    /// <summary>
    /// Gets or sets the antiforgery service for generating form tokens.
    /// </summary>
    [Inject]
    private IAntiforgery Antiforgery { get; set; } = null!;

    /// <summary>
    /// Gets or sets the HTTP context accessor for the current request.
    /// </summary>
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

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        HttpContext? context = HttpContextAccessor.HttpContext;
        if (context is not null)
        {
            AntiforgeryTokenSet tokens = Antiforgery.GetAndStoreTokens(context);
            AntiforgeryRequestToken = tokens.RequestToken;
        }
    }

    /// <summary>
    /// Gets the user-facing error message based on the error query parameter.
    /// </summary>
    private string? ErrorMessage =>
        Error switch
        {
            "invalid" => "Invalid username or password.",
            "failed" => "Login failed. Ensure the Auth service is running.",
            _ => null
        };
}

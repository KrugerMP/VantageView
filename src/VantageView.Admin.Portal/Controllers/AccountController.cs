using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using VantageView.Admin.Portal.Models;

namespace VantageView.Admin.Portal.Controllers;

/// <summary>
/// Handles sign-in and sign-out for the admin portal.
/// </summary>
public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AccountController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    public AccountController(
        IHttpClientFactory httpClientFactory,
        ILogger<AccountController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Signs out the user and returns HTML that redirects to login after the browser has applied the cookie clear.
    /// </summary>
    [HttpGet]
    [Route("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Content(
            """
            <!DOCTYPE html>
            <html><head><meta charset="utf-8"><title>Signing out…</title></head>
            <body><p>Signing out…</p><script>window.location.replace("/login");</script></body>
            </html>
            """,
            "text/html");
    }

    /// <summary>
    /// Accepts login form POST, validates credentials with the Auth API, and signs in with a cookie.
    /// </summary>
    [HttpPost]
    [Route("account/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync([FromForm] string? username, [FromForm] string? password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return Redirect("/login?error=invalid");
        }

        try
        {
            HttpClient client = _httpClientFactory.CreateClient("Auth");
            using HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/auth/login",
                new { Username = username, Password = password });
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Login failed. Invalid username or password.");
                return Redirect("/login?error=invalid");
            }

            LoginResponse? loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (string.IsNullOrEmpty(loginResponse?.Token))
            {
                _logger.LogWarning("Auth API returned no token.");
                return Redirect("/login?error=failed");
            }

            List<Claim> claims =
            [
                new Claim("username", username),
                new Claim("access_token", loginResponse.Token),
            ];
            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                });

            _logger.LogInformation("Login successful. Redirecting to articles page.");
            return Redirect("/articles");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed. Ensure the Auth service is running.");
            return Redirect("/login?error=failed");
        }
    }
}

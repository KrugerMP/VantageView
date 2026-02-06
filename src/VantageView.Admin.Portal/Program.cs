using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VantageView.Admin.Portal.Components;
using VantageView.Admin.Portal.Models;
using VantageView.Admin.Portal.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7021";
string authBaseUrl = builder.Configuration["AuthBaseUrl"] ?? "https://localhost:7128";

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthHandler>();
builder.Services.AddHttpClient("Api", client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<AuthHandler>();
builder.Services.AddHttpClient("Auth", client => client.BaseAddress = new Uri(authBaseUrl));

builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.Name = "VantageView.Admin.Portal.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict; // or Lax

        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured"))),
            ClockSkew = TimeSpan.FromMinutes(5)
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                ILogger<JwtBearerOptions>? logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerOptions>>();
                logger?.LogWarning(context.Exception, "JWT authentication failed: {Message}", context.Exception.Message);
                if (context.Exception is SecurityTokenExpiredException)
                    context.Response.Headers.Append("X-Token-Expired", "true");
                return Task.CompletedTask;
            }
        };
    });

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();

// Cookie sign-in must run in a real HTTP request. This endpoint handles form POST from the login page.
app.MapPost("/account/login", async (
    HttpContext context,
    IFormCollection form,
    IHttpClientFactory httpClientFactory,
    IAntiforgery antiforgery,
    ILogger<Program> logger) =>
{
    if (!await antiforgery.IsRequestValidAsync(context))
    {
        return Results.BadRequest();
    }

    string? username = form["Username"];
    string? password = form["Password"];
    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
    {
        return Results.Redirect("/login?error=invalid");
    }

    try
    {
        HttpClient client = httpClientFactory.CreateClient("Auth");
        using HttpResponseMessage response = await client.PostAsJsonAsync("api/auth/login", new { Username = username, Password = password });
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Login failed. Invalid username or password.");
            return Results.Redirect("/login?error=invalid");
        }

        LoginResponse? loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (string.IsNullOrEmpty(loginResponse?.Token))
        {
            logger.LogWarning("Auth API returned no token.");
            return Results.Redirect("/login?error=failed");
        }

        List<Claim> claims =
        [
            new Claim("username", username),
            new Claim("access_token", loginResponse.Token)
        ];
        ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        ClaimsPrincipal principal = new(identity);

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });
        
        logger.LogInformation("Login successful. Redirecting to articles page.");
        return Results.Redirect("/articles");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Login failed. Ensure the Auth service is running.");
        return Results.Redirect("/login?error=failed");
    }
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

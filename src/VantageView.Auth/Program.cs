using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using VantageView.Auth.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host is "localhost" or "127.0.0.1")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

WebApplication app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/auth/login", (LoginRequest request) =>
{
    // Simplified auth for demo: accept admin/admin
    if (request.Username != "admin" || request.Password != "admin")
    {
        return Results.Unauthorized();
    }

    string key = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
    string issuer = builder.Configuration["Jwt:Issuer"] ?? "VantageView.Auth";
    string audience = builder.Configuration["Jwt:Audience"] ?? "VantageView.API";

    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
    byte[] tokenKey = Encoding.UTF8.GetBytes(key);
    SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "Admin")
        }),
        Expires = DateTime.UtcNow.AddHours(24),
        Issuer = issuer,
        Audience = audience,
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(tokenKey),
            SecurityAlgorithms.HmacSha256Signature)
    };

    SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
    string tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(new LoginResponse(tokenString));
})
.WithName("Login")
.WithTags("Auth");

app.Run();

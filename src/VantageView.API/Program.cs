using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VantageView.API.Domain.AdminArticles.Commands;
using VantageView.API.Domain.Articles.Queries;
using VantageView.API.Domain.Health.Queries;
using VantageView.API.Middleware;
using VantageView.API.Models;
using VantageView.Data;
using VantageView.Data.Entities;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host is "localhost" or "127.0.0.1")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            Dictionary<string, string[]> errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors.Select(err => err.ErrorMessage).ToArray());

            ValidationErrorResponse response = new(
                Status: StatusCodes.Status400BadRequest,
                Message: "One or more validation errors occurred.",
                Errors: errors);

            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAdminArticlesCommands, AdminArticlesCommands>();
builder.Services.AddScoped<IHealthQueries, HealthQueries>();
builder.Services.AddScoped<IArticleQueries, ArticleQueries>();

WebApplication app = builder.Build();

// Apply migrations and seed sample data when database is empty
using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    if (!db.Articles.Any())
    {
        DateTime now = DateTime.UtcNow;
        db.Articles.AddRange(
            new Article
            {
                Id = Guid.NewGuid(),
                Title = "Welcome to VantageView",
                Summary = "Your corporate news system is ready.",
                Content = $"This is the first article. Edit or delete it from the Admin Portal.{Environment.NewLine}If you are seeing this article, it means everything is running",
                Author = "System",
                PublishedAt = now,
                CreatedAt = now,
                UpdatedAt = now
            });
        db.SaveChanges();
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

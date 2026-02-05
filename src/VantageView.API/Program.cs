using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VantageView.API.Models;
using VantageView.Data;
using VantageView.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured")))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5*", "https://localhost:7*")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

// Public endpoints
app.MapGet("/api/articles", async (AppDbContext db, CancellationToken ct) =>
{
    var articles = await db.Articles
        .OrderByDescending(a => a.PublishedAt)
        .Select(a => new ArticleListItemDto(a.Id, a.Title, a.Summary, a.Author, a.PublishedAt))
        .ToListAsync(ct);
    return Results.Ok(articles);
})
.WithName("GetArticles")
.WithTags("Articles");

app.MapGet("/api/articles/{id:int}", async (int id, AppDbContext db, CancellationToken ct) =>
{
    var article = await db.Articles.FindAsync([id], ct);
    return article is null
        ? Results.NotFound()
        : Results.Ok(new ArticleDto(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt));
})
.WithName("GetArticle")
.WithTags("Articles");

// Admin endpoints (require JWT)
app.MapPost("/api/admin/articles", async (CreateArticleDto dto, AppDbContext db, CancellationToken ct) =>
{
    var now = DateTime.UtcNow;
    var publishedAt = dto.PublishedAt ?? now;
    var article = new Article
    {
        Title = dto.Title,
        Summary = dto.Summary,
        Content = dto.Content,
        Author = dto.Author,
        PublishedAt = publishedAt,
        CreatedAt = now,
        UpdatedAt = now
    };
    db.Articles.Add(article);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/articles/{article.Id}", new ArticleDto(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt));
})
.RequireAuthorization()
.WithName("CreateArticle")
.WithTags("Admin");

app.MapPut("/api/admin/articles/{id:int}", async (int id, UpdateArticleDto dto, AppDbContext db, CancellationToken ct) =>
{
    var article = await db.Articles.FindAsync([id], ct);
    if (article is null) return Results.NotFound();
    article.Title = dto.Title;
    article.Summary = dto.Summary;
    article.Content = dto.Content;
    article.Author = dto.Author;
    article.PublishedAt = dto.PublishedAt ?? article.PublishedAt;
    article.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync(ct);
    return Results.Ok(new ArticleDto(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt));
})
.RequireAuthorization()
.WithName("UpdateArticle")
.WithTags("Admin");

app.MapDelete("/api/admin/articles/{id:int}", async (int id, AppDbContext db, CancellationToken ct) =>
{
    var article = await db.Articles.FindAsync([id], ct);
    if (article is null) return Results.NotFound();
    db.Articles.Remove(article);
    await db.SaveChangesAsync(ct);
    return Results.NoContent();
})
.RequireAuthorization()
.WithName("DeleteArticle")
.WithTags("Admin");

app.Run();

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured")))
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

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
            new Article { Title = "Welcome to VantageView", Summary = "Your corporate news system is ready.", Content = "This is the first article. Edit or delete it from the Admin Portal.", Author = "System", PublishedAt = now, CreatedAt = now, UpdatedAt = now },
            new Article { Title = "Getting Started", Summary = "Learn how to manage your news.", Content = "Use the Admin Portal to create, edit, and delete articles. Log in with admin/admin.", Author = "Admin", PublishedAt = now.AddDays(-1), CreatedAt = now, UpdatedAt = now });
        db.SaveChanges();
    }
}

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
    List<ArticleListItemDto> articles = await db.Articles
        .OrderByDescending(a => a.PublishedAt)
        .Select(a => new ArticleListItemDto(a.Id, a.Title, a.Summary, a.Author, a.PublishedAt))
        .ToListAsync(ct);
    return Results.Ok(articles);
})
.WithName("GetArticles")
.WithTags("Articles");

app.MapGet("/api/articles/{id:int}", async (int id, AppDbContext db, CancellationToken ct) =>
{
    Article? article = await db.Articles.FindAsync([id], ct);
    return article is null
        ? Results.NotFound()
        : Results.Ok(new ArticleDto(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt));
})
.WithName("GetArticle")
.WithTags("Articles");

// Admin endpoints (require JWT)
app.MapPost("/api/admin/articles", async (CreateArticleDto dto, AppDbContext db, CancellationToken ct) =>
{
    DateTime now = DateTime.UtcNow;
    DateTime publishedAt = dto.PublishedAt ?? now;
    Article article = new Article
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
    Article? article = await db.Articles.FindAsync([id], ct);
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
    Article? article = await db.Articles.FindAsync([id], ct);
    if (article is null) return Results.NotFound();
    db.Articles.Remove(article);
    await db.SaveChangesAsync(ct);
    return Results.NoContent();
})
.RequireAuthorization()
.WithName("DeleteArticle")
.WithTags("Admin");

app.Run();

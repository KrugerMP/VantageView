using System.Text.Json;
using VantageView.API.Models;
using VantageView.Data;
using VantageView.Data.Entities;

namespace VantageView.API.Domain.AdminArticles.Commands;

/// <summary>
/// Implements admin article commands (create, update, delete) with history snapshots.
/// </summary>
public class AdminArticlesCommands : IAdminArticlesCommands
{
    private readonly AppDbContext _db;
    private readonly ILogger<AdminArticlesCommands> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminArticlesCommands"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="logger">The logger for diagnostics.</param>
    public AdminArticlesCommands(AppDbContext db, ILogger<AdminArticlesCommands> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<(bool success, ArticleDto? result)> CreateArticleAsync(CreateArticleDto createArticleDto, CancellationToken ct)
    {
        try
        {
            DateTime now = DateTime.UtcNow;
            DateTime publishedAt = createArticleDto.PublishedAt ?? now;

            Article article = new()
            {
                Id = Guid.NewGuid(),
                Title = createArticleDto.Title,
                Summary = createArticleDto.Summary,
                Content = createArticleDto.Content,
                Author = createArticleDto.Author,
                PublishedAt = publishedAt,
                CreatedAt = now,
                UpdatedAt = now
            };
            _db.Articles.Add(article);
            await _db.SaveChangesAsync(ct);

            ArticleDto result = new(article.Id, article.Title, article.Summary, article.Content, article.Author,
                article.PublishedAt, article.UpdatedAt);

            return (true, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when trying to create article:{ArticleFull}", JsonSerializer.Serialize(createArticleDto));
            return (false, null);
        }
    }

    /// <inheritdoc />
    public async Task<(bool found, ArticleDto? result)> UpdateArticleAsync(Guid id, UpdateArticleDto dto, CancellationToken ct)
    {
        try
        {
            Article? article = await _db.Articles.FindAsync([id], ct);
            if (article is null)
            {
                return (false, null);
            }

            DateTime now = DateTime.UtcNow;
            _db.ArticleHistory.Add(new ArticleHistory
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                Title = article.Title,
                Summary = article.Summary,
                Content = article.Content,
                Author = article.Author,
                PublishedAt = article.PublishedAt,
                RecordedAt = now
            });

            article.Title = dto.Title;
            article.Summary = dto.Summary;
            article.Content = dto.Content;
            article.UpdatedAt = now;

            await _db.SaveChangesAsync(ct);

            ArticleDto updatedDto = new(article.Id, article.Title, article.Summary, article.Content, article.Author,
                article.PublishedAt, article.UpdatedAt);
            return (true, updatedDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when trying to update article {ArticleId}", id);
            return (false, null);
        }
    }

    /// <inheritdoc />
    public async Task<(bool found, ArticleDto? result)> DeleteArticleAsync(Guid id, CancellationToken ct)
    {
        try
        {
            Article? article = await _db.Articles.FindAsync([id], ct);
            if (article is null)
            {
                return (false, null);
            }

            ArticleDto deletedDto = new(article.Id, article.Title, article.Summary, article.Content, article.Author,
                article.PublishedAt, article.UpdatedAt);

            _db.ArticleHistory.Add(new ArticleHistory
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                Title = article.Title,
                Summary = article.Summary,
                Content = article.Content,
                Author = article.Author,
                PublishedAt = article.PublishedAt,
                RecordedAt = DateTime.UtcNow
            });

            _db.Articles.Remove(article);
            await _db.SaveChangesAsync(ct);

            return (true, deletedDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when trying to delete article {ArticleId}", id);
            return (false, null);
        }
    }
}
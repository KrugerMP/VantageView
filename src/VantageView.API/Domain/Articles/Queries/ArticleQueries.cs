using Microsoft.EntityFrameworkCore;
using VantageView.API.Models;
using VantageView.Data;
using VantageView.Data.Entities;

namespace VantageView.API.Domain.Articles.Queries;

/// <summary>
/// Provides read queries for news articles (list and by ID).
/// </summary>
public class ArticleQueries : IArticleQueries
{
    private readonly AppDbContext _db;
    private readonly ILogger<ArticleQueries> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArticleQueries"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="logger">The logger for diagnostics.</param>
    public ArticleQueries(AppDbContext db, ILogger<ArticleQueries> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Gets all articles ordered by publication date (newest first).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of article list DTOs, or null on error.</returns>
    public async Task<List<ArticleListItemDto>> GetAllArticlesAsync(CancellationToken ct)
    {
        try
        {
            List<ArticleListItemDto> articles = await _db.Articles
                .OrderByDescending(a => a.PublishedAt)
                .Select(a => new ArticleListItemDto(a.Id, a.Title, a.Summary, a.Author, a.PublishedAt, a.UpdatedAt))
                .ToListAsync(ct);

            return articles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when getting all articles");
            return null!;
        }
    }

    /// <summary>
    /// Gets a single article by ID.
    /// </summary>
    /// <param name="id">The article identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The article DTO if found; otherwise null.</returns>
    public async Task<ArticleDto?> GetArticlesByIdAsync(Guid id, CancellationToken ct)
    {
        try
        {
            Article? article = await _db.Articles.FindAsync([id], ct);

            if (article is null)
            {
                return null!;
            }

            return new ArticleDto(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt,
                article.UpdatedAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when getting article by id {ArticleId}", id);
            return null!;
        }
    }
}
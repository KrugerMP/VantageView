using VantageView.API.Models;

namespace VantageView.API.Domain.Articles.Queries;

/// <summary>
/// Defines read queries for news articles.
/// </summary>
public interface IArticleQueries
{
    /// <summary>
    /// Gets all articles ordered by publication date (newest first).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of article list DTOs.</returns>
    Task<List<ArticleListItemDto>> GetAllArticlesAsync(CancellationToken ct);

    /// <summary>
    /// Gets a single article by ID.
    /// </summary>
    /// <param name="id">The article identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The article DTO if found; otherwise null.</returns>
    Task<ArticleDto?> GetArticlesByIdAsync(Guid id, CancellationToken ct);
}
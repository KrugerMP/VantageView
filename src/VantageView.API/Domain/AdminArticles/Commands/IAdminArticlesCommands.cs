using VantageView.API.Models;

namespace VantageView.API.Domain.AdminArticles.Commands;

/// <summary>
/// Commands for admin article operations (create, update, delete).
/// </summary>
public interface IAdminArticlesCommands
{
    /// <summary>
    /// Creates a new article.
    /// </summary>
    /// <param name="createArticleDto">The article data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A tuple of (success, created article DTO). Item2 is null when success is false.</returns>
    Task<(bool success, ArticleDto? result)> CreateArticleAsync(CreateArticleDto createArticleDto, CancellationToken ct);

    /// <summary>
    /// Updates an existing article.
    /// </summary>
    /// <param name="id">The article ID.</param>
    /// <param name="dto">The updated article data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A tuple of (found, updated article DTO). Item2 is null when article was not found.</returns>
    Task<(bool found, ArticleDto? result)> UpdateArticleAsync(Guid id, UpdateArticleDto dto, CancellationToken ct);

    /// <summary>
    /// Deletes an article and records a snapshot in history.
    /// </summary>
    /// <param name="id">The article ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A tuple of (found, deleted article DTO for response). Item2 is null when article was not found.</returns>
    Task<(bool found, ArticleDto? result)> DeleteArticleAsync(Guid id, CancellationToken ct);
}
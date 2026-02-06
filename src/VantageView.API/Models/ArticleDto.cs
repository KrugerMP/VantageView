namespace VantageView.API.Models;

/// <summary>
/// Full article data transfer object.
/// </summary>
/// <param name="Id">The article identifier.</param>
/// <param name="Title">The article title.</param>
/// <param name="Summary">The brief summary.</param>
/// <param name="Content">The full content.</param>
/// <param name="Author">The author name.</param>
/// <param name="PublishedAt">The publication date.</param>
/// <param name="UpdatedAt">The last update timestamp.</param>
public record ArticleDto(
    Guid Id,
    string Title,
    string Summary,
    string Content,
    string Author,
    DateTime PublishedAt,
    DateTime UpdatedAt);

/// <summary>
/// Abbreviated article data for list views.
/// </summary>
/// <param name="Id">The article identifier.</param>
/// <param name="Title">The article title.</param>
/// <param name="Summary">The brief summary.</param>
/// <param name="Author">The author name.</param>
/// <param name="PublishedAt">The publication date.</param>
/// <param name="UpdatedAt">The last update timestamp.</param>
public record ArticleListItemDto(
    Guid Id,
    string Title,
    string Summary,
    string Author,
    DateTime PublishedAt,
    DateTime UpdatedAt);

/// <summary>
/// Data transfer object for creating a new article.
/// </summary>
/// <param name="Title">The article title.</param>
/// <param name="Summary">The brief summary.</param>
/// <param name="Content">The full content.</param>
/// <param name="Author">The author name.</param>
/// <param name="PublishedAt">The publication date (optional; defaults to now).</param>
public record CreateArticleDto(
    string Title,
    string Summary,
    string Content,
    string Author,
    DateTime? PublishedAt);

/// <summary>
/// Data transfer object for updating an existing article.
/// </summary>
/// <param name="Title">The article title.</param>
/// <param name="Summary">The brief summary.</param>
/// <param name="Content">The full content.</param>
public record UpdateArticleDto(
    string Title,
    string Summary,
    string Content);

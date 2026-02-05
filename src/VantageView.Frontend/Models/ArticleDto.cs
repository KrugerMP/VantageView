namespace VantageView.Frontend.Models;

/// <summary>
/// Full article data transfer object.
/// </summary>
/// <param name="Id">The article identifier.</param>
/// <param name="Title">The article title.</param>
/// <param name="Summary">The brief summary.</param>
/// <param name="Content">The full content.</param>
/// <param name="Author">The author name.</param>
/// <param name="PublishedAt">The publication date.</param>
public record ArticleDto(
    int Id,
    string Title,
    string Summary,
    string Content,
    string Author,
    DateTime PublishedAt);

/// <summary>
/// Abbreviated article data for list views.
/// </summary>
/// <param name="Id">The article identifier.</param>
/// <param name="Title">The article title.</param>
/// <param name="Summary">The brief summary.</param>
/// <param name="Author">The author name.</param>
/// <param name="PublishedAt">The publication date.</param>
public record ArticleListItemDto(
    int Id,
    string Title,
    string Summary,
    string Author,
    DateTime PublishedAt);

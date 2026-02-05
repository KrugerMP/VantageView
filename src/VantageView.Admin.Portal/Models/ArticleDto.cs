using System.ComponentModel.DataAnnotations;

namespace VantageView.Admin.Portal.Models;

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

/// <summary>
/// Data transfer object for creating a new article.
/// </summary>
public class CreateArticleDto
{
    /// <summary>
    /// Gets or sets the article title.
    /// </summary>
    [Required]
    public string Title { get; set; } = "";

    /// <summary>
    /// Gets or sets the brief summary.
    /// </summary>
    [Required]
    public string Summary { get; set; } = "";

    /// <summary>
    /// Gets or sets the full article content.
    /// </summary>
    [Required]
    public string Content { get; set; } = "";

    /// <summary>
    /// Gets or sets the author name.
    /// </summary>
    [Required]
    public string Author { get; set; } = "";

    /// <summary>
    /// Gets or sets the publication date (optional; defaults to now when creating).
    /// </summary>
    public DateTime? PublishedAt { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing article.
/// </summary>
public class UpdateArticleDto
{
    /// <summary>
    /// Gets or sets the article title.
    /// </summary>
    [Required]
    public string Title { get; set; } = "";

    /// <summary>
    /// Gets or sets the brief summary.
    /// </summary>
    [Required]
    public string Summary { get; set; } = "";

    /// <summary>
    /// Gets or sets the full article content.
    /// </summary>
    [Required]
    public string Content { get; set; } = "";

    /// <summary>
    /// Gets or sets the author name.
    /// </summary>
    [Required]
    public string Author { get; set; } = "";

    /// <summary>
    /// Gets or sets the publication date (optional).
    /// </summary>
    public DateTime? PublishedAt { get; set; }
}

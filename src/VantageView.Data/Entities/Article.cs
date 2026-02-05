namespace VantageView.Data.Entities;

/// <summary>
/// Represents a corporate news article.
/// </summary>
public class Article
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the article title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the brief summary displayed in article listings.
    /// </summary>
    public required string Summary { get; set; }

    /// <summary>
    /// Gets or sets the full article content.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the author name.
    /// </summary>
    public required string Author { get; set; }

    /// <summary>
    /// Gets or sets the publication date.
    /// </summary>
    public DateTime PublishedAt { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

namespace VantageView.Data.Entities;

/// <summary>
/// Represents a historical snapshot of an article (e.g. before an update or for audit).
/// </summary>
public class ArticleHistory
{
    /// <summary>
    /// Gets or sets the unique identifier for this history record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the article this history record belongs to.
    /// </summary>
    public Guid ArticleId { get; set; }

    /// <summary>
    /// Gets or sets the article title at the time of the snapshot.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the brief summary at the time of the snapshot.
    /// </summary>
    public required string Summary { get; set; }

    /// <summary>
    /// Gets or sets the full content at the time of the snapshot.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the author name at the time of the snapshot.
    /// </summary>
    public required string Author { get; set; }

    /// <summary>
    /// Gets or sets the publication date at the time of the snapshot.
    /// </summary>
    public DateTime PublishedAt { get; set; }

    /// <summary>
    /// Gets or sets when this history record was created (when the snapshot was taken).
    /// </summary>
    public DateTime RecordedAt { get; set; }
}

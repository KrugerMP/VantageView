using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VantageView.API.Models;
using VantageView.Data;
using VantageView.Data.Entities;

namespace VantageView.API.Controllers;

/// <summary>
/// Admin API for creating, updating, and deleting news articles. Requires JWT authentication.
/// </summary>
[ApiController]
[Route("api/admin/articles")]
[Produces("application/json")]
[Authorize]
public class AdminArticlesController : ControllerBase
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminArticlesController"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public AdminArticlesController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Creates a new article.
    /// </summary>
    /// <param name="dto">The article data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created article.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ArticleDto>> CreateArticleAsync([FromBody] CreateArticleDto dto, CancellationToken ct)
    {
        DateTime now = DateTime.UtcNow;
        DateTime publishedAt = dto.PublishedAt ?? now;

        Article article = new()
        {
            Title = dto.Title,
            Summary = dto.Summary,
            Content = dto.Content,
            Author = dto.Author,
            PublishedAt = publishedAt,
            CreatedAt = now,
            UpdatedAt = now
        };
        _db.Articles.Add(article);
        await _db.SaveChangesAsync(ct);

        ArticleDto result = new(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt);
        return CreatedAtAction(nameof(ArticlesController.GetArticleAsync), "Articles", new { id = article.Id }, result);
    }

    /// <summary>
    /// Updates an existing article.
    /// </summary>
    /// <param name="id">The article ID.</param>
    /// <param name="dto">The updated article data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated article, or 404 if not found.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleDto>> UpdateArticleAsync(int id, [FromBody] UpdateArticleDto dto, CancellationToken ct)
    {
        Article? article = await _db.Articles.FindAsync([id], ct);
        if (article is null)
        {
            return NotFound();
        }

        article.Title = dto.Title;
        article.Summary = dto.Summary;
        article.Content = dto.Content;
        article.Author = dto.Author;
        article.PublishedAt = dto.PublishedAt ?? article.PublishedAt;
        article.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return Ok(new ArticleDto(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt));
    }

    /// <summary>
    /// Deletes an article.
    /// </summary>
    /// <param name="id">The article ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 No Content on success, or 404 if not found.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteArticleAsync(int id, CancellationToken ct)
    {
        Article? article = await _db.Articles.FindAsync([id], ct);
        if (article is null)
        {
            return NotFound();
        }

        _db.Articles.Remove(article);
        await _db.SaveChangesAsync(ct);
        
        return NoContent();
    }
}

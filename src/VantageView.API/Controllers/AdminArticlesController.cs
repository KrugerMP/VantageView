using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VantageView.API.Models;
using VantageView.API.Validations;
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
    private readonly ILogger<AdminArticlesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminArticlesController"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="logger">The logger for diagnostics.</param>
    public AdminArticlesController(AppDbContext db, ILogger<AdminArticlesController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new article.
    /// </summary>
    /// <param name="dto">The article data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created article.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ArticleDto>> CreateArticleAsync([FromBody, Required] CreateArticleDto dto, CancellationToken ct)
    {
        try
        {
            CreateArticleDtoValidator createArticleDtoValidator = new();
            FluentValidation.Results.ValidationResult? validationResult = await createArticleDtoValidator.ValidateAsync(dto, ct);

            if (validationResult is null)
            {
                throw new Exception($"Validation result was null for the following request:{JsonSerializer.Serialize(dto)}");
            }

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            DateTime now = DateTime.UtcNow;
            DateTime publishedAt = dto.PublishedAt ?? now;

            Article article = new()
            {
                Id = Guid.NewGuid(),
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

            ArticleDto result = new(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt, article.UpdatedAt);
            return CreatedAtRoute("GetArticle", new { id = article.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating article.");
            return BadRequest();
        }
    }

    /// <summary>
    /// Updates an existing article.
    /// </summary>
    /// <param name="id">The article ID.</param>
    /// <param name="dto">The updated article data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated article, or 404 if not found.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleDto>> UpdateArticleAsync(Guid id, [FromBody, Required] UpdateArticleDto dto, CancellationToken ct)
    {
        try
        {
            Article? article = await _db.Articles.FindAsync([id], ct);
            if (article is null)
            {
                _logger.LogWarning($"Could not find article {id}");
                return NotFound();
            }

            UpdateArticleDtoValidator updateArticleDtoValidator = new();
            FluentValidation.Results.ValidationResult? validationResult = await updateArticleDtoValidator.ValidateAsync(dto, ct);

            if (validationResult is null)
            {
                throw new Exception($"Validation result was null for the following request:{JsonSerializer.Serialize(dto)}");
            }

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
                
            article.Title = dto.Title;
            article.Summary = dto.Summary;
            article.Content = dto.Content;

            article.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            return Ok(new ArticleDto(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt, article.UpdatedAt));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating article {ArticleId}.", id);
            return BadRequest();
        }
    }

    /// <summary>
    /// Deletes an article.
    /// </summary>
    /// <param name="id">The article ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>204 No Content on success, or 404 if not found.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteArticleAsync([Required] Guid id, CancellationToken ct)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting article {ArticleId}.", id);
            return BadRequest();
        }
    }
}

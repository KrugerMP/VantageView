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
    [ProducesResponseType(typeof(BaseResponseModel<ArticleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponseModel<ArticleDto>>> CreateArticleAsync([FromBody, Required] CreateArticleDto dto,
        CancellationToken ct)
    {
        try
        {
            CreateArticleDtoValidator createArticleDtoValidator = new();
            FluentValidation.Results.ValidationResult? validationResult =
                await createArticleDtoValidator.ValidateAsync(dto, ct);

            if (validationResult is null)
            {
                throw new Exception(
                    $"Validation result was null for the following request:{JsonSerializer.Serialize(dto)}");
            }

            if (!validationResult.IsValid)
            {
                IReadOnlyDictionary<string, string[]> errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(new ValidationErrorResponse(
                    StatusCodes.Status400BadRequest,
                    "One or more validation errors occurred.",
                    errors));
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

            ArticleDto result = new(article.Id, article.Title, article.Summary, article.Content, article.Author,
                article.PublishedAt, article.UpdatedAt);
            return CreatedAtRoute("GetArticle", new { id = article.Id }, new BaseResponseModel<ArticleDto>
            {
                Result = result,
                Message = "Article created",
                ResponseTime = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating article.");
            return BadRequest(new BaseResponseModel<ArticleDto>
            {
                Error = new ErrorResponseModel { Message = "Error creating article." },
                ResponseTime = DateTime.UtcNow,
                Message = "An error occurred when creating the article" 
            });
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
    [ProducesResponseType(typeof(BaseResponseModel<ArticleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponseModel<ArticleDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponseModel<ArticleDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResponseModel<ArticleDto>>> UpdateArticleAsync(Guid id, [FromBody, Required] UpdateArticleDto dto,
        CancellationToken ct)
    {
        try
        {
            Article? article = await _db.Articles.FindAsync([id], ct);
            if (article is null)
            {
                _logger.LogWarning("Could not find article {ArticleId}", id);
                return NotFound(new BaseResponseModel<ArticleDto>
                {
                    Error = new ErrorResponseModel { Message = "Could not find article to update." },
                    Message = "Error processing request",
                    ResponseTime = DateTime.UtcNow,
                    Result = null!
                });
            }

            UpdateArticleDtoValidator updateArticleDtoValidator = new();
            FluentValidation.Results.ValidationResult? validationResult =
                await updateArticleDtoValidator.ValidateAsync(dto, ct);

            if (validationResult is null)
            {
                throw new Exception(
                    $"Validation result was null for the following request:{JsonSerializer.Serialize(dto)}");
            }

            if (!validationResult.IsValid)
            {
                IReadOnlyDictionary<string, string[]> errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                return BadRequest(new ValidationErrorResponse(
                    StatusCodes.Status400BadRequest,
                    "One or more validation errors occurred.",
                    errors));
            }

            DateTime now = DateTime.UtcNow;
            _db.ArticleHistory.Add(new ArticleHistory
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                Title = article.Title,
                Summary = article.Summary,
                Content = article.Content,
                Author = article.Author,
                PublishedAt = article.PublishedAt,
                RecordedAt = now
            });

            article.Title = dto.Title;
            article.Summary = dto.Summary;
            article.Content = dto.Content;
            article.UpdatedAt = now;

            await _db.SaveChangesAsync(ct);

            ArticleDto updatedDto = new(article.Id, article.Title, article.Summary, article.Content, article.Author,
                article.PublishedAt, article.UpdatedAt);
            return Ok(new BaseResponseModel<ArticleDto>
            {
                Result = updatedDto,
                Message = "Article updated",
                ResponseTime = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating article {ArticleId}.", id);
            return BadRequest(new BaseResponseModel<ArticleDto>
            {
                Error = new ErrorResponseModel { Message = "Error updating article." },
                ResponseTime = DateTime.UtcNow,
                Message = "Error updating article."
            });
        }
    }

    /// <summary>
    /// Deletes an article.
    /// </summary>
    /// <param name="id">The article ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>200 OK with message on success, or 404 if not found.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponseModel<ArticleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponseModel<ArticleDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponseModel<ArticleDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResponseModel<Article>>> DeleteArticleAsync([Required] Guid id, CancellationToken ct)
    {
        try
        {
            Article? article = await _db.Articles.FindAsync([id], ct);

            if (article is null)
            {
                _logger.LogWarning("Could not find article {ArticleId}", id);
                return NotFound(new BaseResponseModel<ArticleDto>
                {
                    Error = new ErrorResponseModel { Message = "Could not find article to delete." },
                    Message = "Error processing request",
                    ResponseTime = DateTime.UtcNow,
                    Result = null!
                });
            }

            _db.ArticleHistory.Add(new ArticleHistory
            {
                Id = Guid.NewGuid(),
                ArticleId = article.Id,
                Title = article.Title,
                Summary = article.Summary,
                Content = article.Content,
                Author = article.Author,
                PublishedAt = article.PublishedAt,
                RecordedAt = DateTime.UtcNow
            });

            _db.Articles.Remove(article);
            await _db.SaveChangesAsync(ct);

            ArticleDto dto = new(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt, article.UpdatedAt);

            return Ok(new BaseResponseModel<ArticleDto>
            {
                Message = "Article deleted",
                ResponseTime = DateTime.UtcNow,
                Result = dto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting article {ArticleId}.", id);
            return BadRequest(new BaseResponseModel<ArticleDto>
            {
                Error = new ErrorResponseModel { Message = "Error deleting article." },
                ResponseTime = DateTime.UtcNow,
                Message = "Error deleting article."
            });
        }
    }
}
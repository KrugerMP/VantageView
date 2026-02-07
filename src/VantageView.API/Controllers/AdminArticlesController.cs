using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VantageView.API.Domain.AdminArticles.Commands;
using VantageView.API.Domain.Articles.Queries;
using VantageView.API.Models;
using VantageView.API.Validations;

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
    private readonly ILogger<AdminArticlesController> _logger;
    private readonly IAdminArticlesCommands _adminArticlesCommands;

    private readonly IArticleQueries _articleQueries;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminArticlesController"/> class.
    /// </summary>
    /// <param name="logger">The logger for diagnostics.</param>
    /// <param name="adminArticlesCommands">The admin article commands (create, update, delete).</param>
    public AdminArticlesController(ILogger<AdminArticlesController> logger, IAdminArticlesCommands adminArticlesCommands, IArticleQueries articleQueries)
    {
        _logger = logger;
        _adminArticlesCommands = adminArticlesCommands;
        _articleQueries = articleQueries;
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
    public async Task<IActionResult> CreateArticleAsync([FromBody, Required] CreateArticleDto dto,
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

            (bool success, ArticleDto? result) = await _adminArticlesCommands.CreateArticleAsync(dto, ct);

            if (!success || result is null)
            {
                throw new Exception("Could not create article");
            }

            return CreatedAtRoute("GetArticle", new { id = result.Id }, new BaseResponseModel<ArticleDto>
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
    public async Task<IActionResult> UpdateArticleAsync(Guid id, [FromBody, Required] UpdateArticleDto dto,
        CancellationToken ct)
    {
        try
        {
            ArticleDto? article = await _articleQueries.GetArticlesByIdAsync(id, ct);
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

            (bool found, ArticleDto? result) = await _adminArticlesCommands.UpdateArticleAsync(id, dto, ct);

            if (!found || result is null)
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

            return Ok(new BaseResponseModel<ArticleDto>
            {
                Result = result,
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
    public async Task<IActionResult> DeleteArticleAsync([Required] Guid id, CancellationToken ct)
    {
        try
        {
            (bool found, ArticleDto? result) = await _adminArticlesCommands.DeleteArticleAsync(id, ct);

            if (!found)
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

            return Ok(new BaseResponseModel<ArticleDto>
            {
                Message = "Article deleted",
                ResponseTime = DateTime.UtcNow,
                Result = result!
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
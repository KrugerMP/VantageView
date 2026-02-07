using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VantageView.API.Domain.Articles.Queries;
using VantageView.API.Models;
using VantageView.Data;
using VantageView.Data.Entities;

namespace VantageView.API.Controllers;

/// <summary>
/// Public API for listing and viewing news articles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleQueries _articleQueries;
    private readonly ILogger<ArticlesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlesController"/> class.
    /// </summary>
    /// <param name="articleQueries">The article query service.</param>
    /// <param name="logger">The logger for diagnostics.</param>
    public ArticlesController(IArticleQueries articleQueries, ILogger<ArticlesController> logger)
    {
        _articleQueries = articleQueries;
        _logger = logger;
    }

    /// <summary>
    /// Gets all articles, ordered by publication date (newest first).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of article summaries.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponseModel<List<ArticleListItemDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponseModel<List<ArticleListItemDto>>>> GetArticlesAsync(CancellationToken ct)
    {
        try
        {
            List<ArticleListItemDto> articles = await _articleQueries.GetAllArticlesAsync(ct);

            return Ok(new BaseResponseModel<List<ArticleListItemDto>>
            {
                Result = articles,
                ResponseTime = DateTime.UtcNow,
                Message = "Articles retrieved successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching articles.");
            return BadRequest(new BaseResponseModel<List<ArticleListItemDto>>
            {
                Error = new ErrorResponseModel { Message = "Error fetching articles." },
                ResponseTime = DateTime.UtcNow,
                Message = "Error fetching articles."
            });
        }
    }

    /// <summary>
    /// Gets a single article by ID.
    /// </summary>
    /// <param name="id">The article ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The article if found; otherwise 404.</returns>
    [HttpGet("{id:guid}", Name = "GetArticle")]
    [ProducesResponseType(typeof(BaseResponseModel<ArticleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponseModel<ArticleDto>>> GetArticleAsync(Guid id, CancellationToken ct)
    {
        try
        {
            ArticleDto? dto = await _articleQueries.GetArticlesByIdAsync(id, ct);

            if (dto is null)
            {
                return NotFound(new BaseResponseModel<ArticleDto>
                {
                    Error = new ErrorResponseModel { Message = "Article not found." },
                    ResponseTime = DateTime.UtcNow,
                    Result = null!,
                    Message = "Article not found."
                });
            }

            return Ok(new BaseResponseModel<ArticleDto>
            {
                Result = dto,
                ResponseTime = DateTime.UtcNow,
                Message = "Article retrieved successfully."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching article {ArticleId}.", id);
            return BadRequest(new BaseResponseModel<ArticleDto>
            {
                Error = new ErrorResponseModel { Message = "Error fetching article." },
                ResponseTime = DateTime.UtcNow,
                Message = "Error fetching article."
            });
        }
    }
}

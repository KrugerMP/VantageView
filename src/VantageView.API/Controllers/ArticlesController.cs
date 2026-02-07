using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private readonly AppDbContext _db;
    private readonly ILogger<ArticlesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlesController"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="logger">The logger for diagnostics.</param>
    public ArticlesController(AppDbContext db, ILogger<ArticlesController> logger)
    {
        _db = db;
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
            List<ArticleListItemDto> articles = await _db.Articles
                .OrderByDescending(a => a.PublishedAt)
                .Select(a => new ArticleListItemDto(a.Id, a.Title, a.Summary, a.Author, a.PublishedAt, a.UpdatedAt))
                .ToListAsync(ct);

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
            Article? article = await _db.Articles.FindAsync([id], ct);
            if (article is null)
            {
                return NotFound(new BaseResponseModel<ArticleDto>
                {
                    Error = new ErrorResponseModel { Message = "Article not found." },
                    ResponseTime = DateTime.UtcNow,
                    Result = null!,
                    Message = "Article not found."
                });
            }

            ArticleDto dto = new(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt, article.UpdatedAt);
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

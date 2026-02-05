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

    /// <summary>
    /// Initializes a new instance of the <see cref="ArticlesController"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public ArticlesController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Gets all articles, ordered by publication date (newest first).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of article summaries.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ArticleListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ArticleListItemDto>>> GetArticlesAsync(CancellationToken ct)
    {
        List<ArticleListItemDto> articles = await _db.Articles
            .OrderByDescending(a => a.PublishedAt)
            .Select(a => new ArticleListItemDto(a.Id, a.Title, a.Summary, a.Author, a.PublishedAt))
            .ToListAsync(ct);
        return Ok(articles);
    }

    /// <summary>
    /// Gets a single article by ID.
    /// </summary>
    /// <param name="id">The article ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The article if found; otherwise 404.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleDto>> GetArticleAsync(int id, CancellationToken ct)
    {
        Article? article = await _db.Articles.FindAsync([id], ct);
        if (article is null)
        {
            return NotFound();
        }
        return Ok(new ArticleDto(article.Id, article.Title, article.Summary, article.Content, article.Author, article.PublishedAt));
    }
}

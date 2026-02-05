using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VantageView.Data;
using VantageView.Data.Entities;
using VantageView.API.Models;

namespace VantageView.API.Controllers;

/// <summary>
/// Health check endpoints for monitoring API and service availability.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthController"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public HealthController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Checks that the articles service and database are reachable.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>200 OK if the service is healthy; 400 Bad Request if no articles exist.</returns>
    [HttpGet("articles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HealthDto>> GetArticlesServiceHealthAsync(CancellationToken ct)
    {
        Article? article = await _db.Articles.FirstOrDefaultAsync(ct);

        if (article is null)
        {
            return BadRequest(new HealthDto { IsHealthy = false, Message = "No articles found" });
        }

        return Ok(new HealthDto { IsHealthy = true, Message = "Articles service is healthy" });
    }
}

using VantageView.Data;
using VantageView.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace VantageView.API.Domain.Health.Queries;

/// <summary>
/// Provides health-check queries for the articles service (e.g. database reachability).
/// </summary>
public class HealthQueries : IHealthQueries
{
    private readonly AppDbContext _db;
    private readonly ILogger<HealthQueries> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthQueries"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="logger">The logger for health check diagnostics.</param>
    public HealthQueries(AppDbContext db, ILogger<HealthQueries> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Determines whether the articles service is healthy (database reachable and at least one article exists).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if the service is healthy; otherwise <c>false</c>.</returns>
    public async Task<bool> IsHealthyAsync(CancellationToken ct)
    {
        try
        {
            Article? article = await _db.Articles.FirstOrDefaultAsync(ct);

            return article is not null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred when trying to validate system health.");
            return false;
        }
    }
}
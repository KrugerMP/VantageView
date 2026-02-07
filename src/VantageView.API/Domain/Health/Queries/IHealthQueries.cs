namespace VantageView.API.Domain.Health.Queries;

/// <summary>
/// Defines health-check queries for the articles service.
/// </summary>
public interface IHealthQueries
{
    /// <summary>
    /// Determines whether the articles service is healthy (database reachable and at least one article exists).
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns><c>true</c> if the service is healthy; otherwise <c>false</c>.</returns>
    Task<bool> IsHealthyAsync(CancellationToken ct);
}
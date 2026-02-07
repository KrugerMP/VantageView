using Microsoft.AspNetCore.Mvc;
using VantageView.API.Models;
using VantageView.API.Domain.Health.Queries;

namespace VantageView.API.Controllers;

/// <summary>
/// Health check endpoints for monitoring API and service availability.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    private readonly IHealthQueries _healthQueries;
    private readonly ILogger<HealthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthController"/> class.
    /// </summary>
    /// <param name="healthQueries">The health check query service.</param>
    /// <param name="logger">The logger for health check diagnostics.</param>
    public HealthController(IHealthQueries healthQueries, ILogger<HealthController> logger)
    {
        _healthQueries = healthQueries;
        _logger = logger;
    }

    /// <summary>
    /// Checks that the articles service and database are reachable.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>200 OK if the service is healthy; 400 Bad Request if no articles exist.</returns>
    [HttpGet("articles")]
    [ProducesResponseType(typeof(BaseResponseModel<HealthDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetArticlesServiceHealthAsync(CancellationToken ct)
    {
        try
        {
            bool healthResult = await _healthQueries.IsHealthyAsync(ct);

            if (!healthResult)
            {
                return BadRequest(new BaseResponseModel<HealthDto>
                {
                    Result = new HealthDto { IsHealthy = false, Message = "No articles found" },
                    Error = new ErrorResponseModel { Message = "No articles found" },
                    ResponseTime = DateTime.UtcNow,
                    Message = "Health of service could not be guaranteed"
                });
            }

            return Ok(new BaseResponseModel<HealthDto>
            {
                Result = new HealthDto { IsHealthy = true, Message = "Articles service is healthy" },
                ResponseTime = DateTime.UtcNow,
                Message = "Successfully processed health request",
                Error = null!
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking articles service health.");
            return BadRequest(new BaseResponseModel<HealthDto>
            {
                Result = new HealthDto { IsHealthy = false, Message = "Error checking articles service health." },
                Error = new ErrorResponseModel { Message = "Error checking articles service health." },
                ResponseTime = DateTime.UtcNow,
                Message = "Error checking articles service health."
            });
        }
    }
}
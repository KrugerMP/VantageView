namespace VantageView.API.Models;

/// <summary>
/// Health check response data.
/// </summary>
public class HealthDto
{
    /// <summary>
    /// Gets or sets a value indicating whether the service is healthy.
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// Gets or sets an optional message describing the health status.
    /// </summary>
    public string Message { get; set; } = "";
}
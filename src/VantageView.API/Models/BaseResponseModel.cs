namespace VantageView.API.Models;

/// <summary>
/// Base shape for API responses, with optional message, timestamp, and error details.
/// </summary>
public class BaseResponseModel<T> where T : class
{
    /// <summary>
    /// Gets or sets a human-readable message describing the response (always present).
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the time at which the response was generated.
    /// </summary>
    public DateTime ResponseTime { get; set; }

    /// <summary>
    /// Gets or sets the response payload (e.g. article, list, or health data).
    /// </summary>
    public T Result { get; set; } = null!;

    /// <summary>
    /// Gets or sets error details when the request failed; otherwise null.
    /// </summary>
    public ErrorResponseModel? Error { get; set; }
}

/// <summary>
/// Error details included in a failed response.
/// </summary>
public class ErrorResponseModel
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public required string Message { get; set; }
}
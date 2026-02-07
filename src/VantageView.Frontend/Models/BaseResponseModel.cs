namespace VantageView.Frontend.Models;

/// <summary>
/// Wrapper for API responses; actual payload is in <see cref="Result"/>.
/// </summary>
public class BaseResponseModel<T>
{
    /// <summary>Human-readable message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>When the response was generated.</summary>
    public DateTime ResponseTime { get; set; }

    /// <summary>The response payload (e.g. article, list).</summary>
    public T? Result { get; set; }

    /// <summary>Error details when the request failed.</summary>
    public ErrorResponseModel? Error { get; set; }
}

/// <summary>Error details in a failed response.</summary>
public class ErrorResponseModel
{
    /// <summary>Error message.</summary>
    public string Message { get; set; } = string.Empty;
}

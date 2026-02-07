namespace VantageView.API.Models;

/// <summary>
/// Custom shape for 400 validation error responses.
/// Customize properties here to change the API response format.
/// </summary>
/// <param name="Status">HTTP status code (e.g. 400).</param>
/// <param name="Message">Human-readable summary of the validation failure.</param>
/// <param name="Errors">Field names and their validation error messages.</param>
public record ValidationErrorResponse(
    int Status,
    string Message,
    IReadOnlyDictionary<string, string[]> Errors);

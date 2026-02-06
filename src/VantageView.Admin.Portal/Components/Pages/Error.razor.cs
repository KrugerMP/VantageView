using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace VantageView.Admin.Portal.Components.Pages;

/// <summary>
/// Error page displayed when an unhandled exception occurs.
/// </summary>
public partial class Error
{
    /// <summary>
    /// Gets or sets the cascading HTTP context for request tracing.
    /// </summary>
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    /// <summary>
    /// Gets or sets the request ID for error diagnostics.
    /// </summary>
    private string? RequestId { get; set; }

    /// <summary>
    /// Gets a value indicating whether a request ID is available to display.
    /// </summary>
    private bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    /// <inheritdoc />
    protected override void OnInitialized() =>
        RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
}

using Microsoft.AspNetCore.Components;
using VantageView.Frontend.Models;

namespace VantageView.Frontend.Components.Pages;

/// <summary>
/// Home page displaying the list of corporate news articles.
/// </summary>
public partial class Home
{
    /// <summary>
    /// Gets or sets the HTTP client factory for API requests.
    /// </summary>
    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = null!;

    /// <summary>
    /// Injects the logging service for audit trails.
    /// </summary>
    [Inject]
    private Logger<Home> Logger { get; set; } = null!;

    /// <summary>
    /// The list of articles fetched from the API.
    /// </summary>
    private List<ArticleListItemDto> articles = [];

    /// <summary>
    /// Whether articles are currently being loaded.
    /// </summary>
    private bool loading = true;

    /// <summary>
    /// Error message to display if loading fails.
    /// </summary>
    private string? error;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        try
        {
            HttpClient client = HttpClientFactory.CreateClient("Api");
            List<ArticleListItemDto>? result = await client.GetFromJsonAsync<List<ArticleListItemDto>>("api/articles");
            articles = result ?? [];
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Could not load all articles at {DateTime.UtcNow}");
            error = "Failed to load articles";
        }
        finally
        {
            loading = false;
        }
    }
}

using System.Net;
using Microsoft.AspNetCore.Components;
using VantageView.Frontend.Models;

namespace VantageView.Frontend.Components.Pages;

/// <summary>
/// Displays the full content of a single news article by ID.
/// </summary>
public partial class Article
{
    /// <summary>
    /// Gets or sets the article identifier from the route.
    /// </summary>
    [Parameter]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the HTTP client factory for API requests.
    /// </summary>
    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = null!;

    /// <summary>
    /// Injects the logging service for audit trails
    /// </summary>
    [Inject]
    private ILogger<Article> Logger { get; set; } = null!;

    /// <summary>
    /// The loaded article data, or null if not found or not yet loaded.
    /// </summary>
    private ArticleDto? article;

    /// <summary>
    /// Whether the article is currently being loaded.
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
            HttpResponseMessage response = await client.GetAsync($"api/articles/{Id}");
            if (response.IsSuccessStatusCode)
            {
                BaseResponseModel<ArticleDto>? wrapper = await response.Content.ReadFromJsonAsync<BaseResponseModel<ArticleDto>>();
                article = wrapper?.Result;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                article = null;
            }
            else
            {
                error = "Failed to load article.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error occurred when loading article:[{Id}] at {DateTime.UtcNow}");
            error = "Failed to load article";
        }
        finally
        {
            loading = false;
        }
    }

    /// <summary>
    /// Converts plain text content to HTML paragraphs. Can be replaced with a Markdown library.
    /// </summary>
    /// <param name="content">The raw text content.</param>
    /// <returns>HTML string with content wrapped in paragraph tags.</returns>
    private static string MarkdownToHtml(string content)
    {
        // Simple plain text to paragraphs - can be replaced with a Markdown library
        string escaped = WebUtility.HtmlEncode(content);
        string[] paragraphs = escaped.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        return string.Join("", paragraphs.Select(p => $"<p>{p.Replace("\n", "<br/>")}</p>"));
    }
}

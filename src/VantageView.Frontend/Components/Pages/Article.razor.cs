using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using VantageView.Frontend.Models;

namespace VantageView.Frontend.Components.Pages;

public partial class Article
{
    [Parameter]
    public Guid Id { get; set; }

    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    private ArticleDto? article;
    private bool loading = true;
    private string? error;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            HttpClient client = HttpClientFactory.CreateClient("Api");
            HttpResponseMessage response = await client.GetAsync($"api/articles/{Id}");
            if (response.IsSuccessStatusCode)
            {
                article = await response.Content.ReadFromJsonAsync<ArticleDto>();
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
            error = $"Failed to load article: {ex.Message}";
        }
        finally
        {
            loading = false;
        }
    }

    private static string MarkdownToHtml(string content)
    {
        // Simple plain text to paragraphs - can be replaced with a Markdown library
        string escaped = WebUtility.HtmlEncode(content);
        string[] paragraphs = escaped.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        return string.Join("", paragraphs.Select(p => $"<p>{p.Replace("\n", "<br/>")}</p>"));
    }
}

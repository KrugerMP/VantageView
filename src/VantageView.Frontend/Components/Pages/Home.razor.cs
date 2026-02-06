using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using VantageView.Frontend.Models;

namespace VantageView.Frontend.Components.Pages;

public partial class Home
{
    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = null!;

    private List<ArticleListItemDto> articles = [];
    private bool loading = true;
    private string? error;

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
            error = $"Failed to load articles: {ex.Message}";
        }
        finally
        {
            loading = false;
        }
    }
}

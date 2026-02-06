using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using VantageView.Admin.Portal.Models;
using VantageView.Admin.Portal.Services;

namespace VantageView.Admin.Portal.Components.Pages;

public partial class Articles
{
    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = null!;

    [Inject]
    private AuthService AuthService { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    private List<ArticleListItemDto> articles = [];
    private ArticleListItemDto? articleToDelete;
    private bool loading = true;
    private string? error;

    protected override async Task OnInitializedAsync()
    {
        if (AuthService.IsAuthenticated)
        {
            await LoadArticles();
        }
        else
        {
            loading = false;
        }
    }

    private async Task LoadArticles()
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

    private void ConfirmDelete(ArticleListItemDto article) => articleToDelete = article;
    private void CancelDelete() => articleToDelete = null;

    private async Task DoDelete()
    {
        if (articleToDelete is null) return;
        try
        {
            HttpClient client = HttpClientFactory.CreateClient("Api");
            HttpResponseMessage response = await client.DeleteAsync($"api/admin/articles/{articleToDelete.Id}");
            if (response.IsSuccessStatusCode)
            {
                articleToDelete = null;
                await LoadArticles();
            }
            else
            {
                error = "Failed to delete article.";
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
    }
}

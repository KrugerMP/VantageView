using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using VantageView.Admin.Portal.Models;
using VantageView.Admin.Portal.Services;

namespace VantageView.Admin.Portal.Components.Pages;

/// <summary>
/// Admin page for managing (listing, editing, deleting) news articles.
/// </summary>
public partial class Articles
{
    /// <summary>
    /// Gets or sets the HTTP client factory for API requests.
    /// </summary>
    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = null!;

    /// <summary>
    /// Gets or sets the authentication service for auth state.
    /// </summary>
    [Inject]
    private AuthService AuthService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation manager for redirects.
    /// </summary>
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    /// <summary>
    /// The list of articles fetched from the API.
    /// </summary>
    private List<ArticleListItemDto> articles = [];

    /// <summary>
    /// The article selected for deletion confirmation, or null.
    /// </summary>
    private ArticleListItemDto? articleToDelete;

    /// <summary>
    /// Whether articles are currently being loaded.
    /// </summary>
    private bool loading = true;

    /// <summary>
    /// Error message to display if an operation fails.
    /// </summary>
    private string? error;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        if (AuthService.IsAuthenticated)
        {
            await LoadArticlesAsync();
        }
        else
        {
            loading = false;
        }
    }

    /// <summary>
    /// Loads the list of articles from the API.
    /// </summary>
    private async Task LoadArticlesAsync()
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

    /// <summary>
    /// Sets the article to delete and shows the confirmation modal.
    /// </summary>
    /// <param name="article">The article to delete.</param>
    private void ConfirmDelete(ArticleListItemDto article) => articleToDelete = article;

    /// <summary>
    /// Cancels the delete confirmation and clears the selection.
    /// </summary>
    private void CancelDelete() => articleToDelete = null;

    /// <summary>
    /// Deletes the selected article via the API and reloads the list.
    /// </summary>
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
                await LoadArticlesAsync();
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

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

    [Inject]
    private ILogger<Articles> Logger { get; set; } = null!;

    /// <summary>
    /// Gets or sets the authentication service for auth state.
    /// </summary>
    [Inject]
    private AuthService AuthService { get; set; } = null!;

    /// <summary>
    /// The list of articles fetched from the API.
    /// </summary>
    private List<ArticleListItemDto> ArticleList { get; set; } = [];

    /// <summary>
    /// The article selected for deletion confirmation, or null.
    /// </summary>
    private ArticleListItemDto? ArticleToDelete { get; set; }

    /// <summary>
    /// Whether articles are currently being loaded.
    /// </summary>
    private bool Loading { get; set; } = true;

    /// <summary>
    /// Error message to display if an operation fails.
    /// </summary>
    private string? Error { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        Logger.LogInformation("Loading articles at {Time}", DateTime.UtcNow);
        Logger.LogInformation("Loading for: {UserName}", AuthService.UserName ?? "(anonymous)");

        if (AuthService.IsAuthenticated)
        {
            await LoadArticlesAsync();
        }
        else
        {
            Loading = false;
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
            ArticleList = result ?? [];
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred when loading articles.");
            Error = "Failed to load articles";
        }
        finally
        {
            Loading = false;
        }
    }

    /// <summary>
    /// Sets the article to delete and shows the confirmation modal.
    /// </summary>
    /// <param name="article">The article to delete.</param>
    private void ConfirmDelete(ArticleListItemDto article) => ArticleToDelete = article;

    /// <summary>
    /// Cancels the delete confirmation and clears the selection.
    /// </summary>
    private void CancelDelete() => ArticleToDelete = null;

    /// <summary>
    /// Deletes the selected article via the API and reloads the list.
    /// </summary>
    private async Task DoDeleteAsync()
    {
        if (ArticleToDelete is null)
        {
            return;
        }

        try
        {
            HttpClient client = HttpClientFactory.CreateClient("Api");
            HttpResponseMessage response = await client.DeleteAsync($"api/admin/articles/{ArticleToDelete.Id}");
            if (response.IsSuccessStatusCode)
            {
                ArticleToDelete = null;
                await LoadArticlesAsync();
            }
            else
            {
                Error = "Failed to delete article.";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not delete article.");
            Error = "Could not delete article, please contact support";
        }
    }
}

using Microsoft.AspNetCore.Components;
using VantageView.Admin.Portal.Models;
using VantageView.Admin.Portal.Services;

namespace VantageView.Admin.Portal.Components.Pages;

/// <summary>
/// Page for creating or editing a news article.
/// </summary>
public partial class ArticleEdit
{
    /// <summary>
    /// Gets or sets the article identifier from the route; Guid.Empty indicates new article.
    /// </summary>
    [Parameter]
    public Guid Id { get; set; }

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
    /// Gets or sets the logger for this component.
    /// </summary>
    [Inject]
    private ILogger<ArticleEdit> Logger { get; set; } = null!;
    
    /// <summary>
    /// Gets a value indicating whether this is a new article (create) vs edit.
    /// </summary>
    private bool IsNew => Id == Guid.Empty;

    /// <summary>
    /// The form model for create or update.
    /// </summary>
    private CreateArticleDto model = new();

    /// <summary>
    /// The selected publish date for the article.
    /// </summary>
    private DateTime _publishDate = DateTime.Today;

    /// <summary>
    /// Whether a save operation is in progress.
    /// </summary>
    private bool loading;

    /// <summary>
    /// Error message to display if save or load fails.
    /// </summary>
    private string? error;

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        if (!AuthService.IsAuthenticated)
        {
            return;
        }

        if (!IsNew)
        {
            await LoadArticleAsync();
        }
        else
        {
            _publishDate = DateTime.Today;
        }
    }

    /// <summary>
    /// Loads the existing article data for edit mode.
    /// </summary>
    private async Task LoadArticleAsync()
    {
        try
        {
            HttpClient client = HttpClientFactory.CreateClient("Api");
            ArticleDto? article = await client.GetFromJsonAsync<ArticleDto>($"api/articles/{Id}");

            if (article is not null)
            {
                model = new CreateArticleDto
                {
                    Title = article.Title,
                    Summary = article.Summary,
                    Content = article.Content,
                    Author = article.Author,
                    PublishedAt = article.PublishedAt
                };
                _publishDate = article.PublishedAt.Date;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Could not load article");
            error = "Article could not be loaded";
        }
    }

    /// <summary>
    /// Handles form submission to create or update the article.
    /// </summary>
    private async Task HandleSaveAsync()
    {
        loading = true;
        error = null;

        try
        {
            HttpClient client = HttpClientFactory.CreateClient("Api");
            model.PublishedAt = _publishDate;

            if (IsNew)
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("api/admin/articles", model);

                if (response.IsSuccessStatusCode)
                {
                    Navigation.NavigateTo("/articles", forceLoad: true);
                }
                else
                {
                    error = "Failed to create article.";
                }
            }
            else
            {
                UpdateArticleDto updateDto = new()
                {
                    Title = model.Title,
                    Summary = model.Summary,
                    Content = model.Content,
                    Author = model.Author,
                    PublishedAt = _publishDate
                };
                HttpResponseMessage response = await client.PutAsJsonAsync($"api/admin/articles/{Id}", updateDto);
                if (response.IsSuccessStatusCode)
                {
                    Navigation.NavigateTo("/articles");
                }
                else
                {
                    error = "Failed to update article.";
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred when saving changes to the article");
             error = "Could not save changes to the article.";
        }
        finally
        {
            loading = false;
        }
    }
}

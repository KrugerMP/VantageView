using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using VantageView.Admin.Portal.Models;
using VantageView.Admin.Portal.Services;

namespace VantageView.Admin.Portal.Components.Pages;

public partial class ArticleEdit
{
    [Parameter]
    public Guid Id { get; set; }

    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = null!;

    [Inject]
    private AuthService AuthService { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    private bool IsNew => Id == Guid.Empty;
    private CreateArticleDto model = new();
    private DateTime publishDate = DateTime.Today;
    private bool loading;
    private string? error;

    protected override async Task OnInitializedAsync()
    {
        if (!AuthService.IsAuthenticated) return;
        if (!IsNew)
        {
            await LoadArticle();
        }
        else
        {
            publishDate = DateTime.Today;
        }
    }

    private async Task LoadArticle()
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
                publishDate = article.PublishedAt.Date;
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
        }
    }

    private async Task HandleSave()
    {
        loading = true;
        error = null;
        try
        {
            HttpClient client = HttpClientFactory.CreateClient("Api");
            model.PublishedAt = publishDate;

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
                UpdateArticleDto updateDto = new UpdateArticleDto
                {
                    Title = model.Title,
                    Summary = model.Summary,
                    Content = model.Content,
                    Author = model.Author,
                    PublishedAt = publishDate
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
            error = ex.Message;
        }
        finally
        {
            loading = false;
        }
    }
}

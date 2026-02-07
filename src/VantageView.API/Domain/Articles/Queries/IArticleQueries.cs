using VantageView.API.Models;

namespace VantageView.API.Domain.Articles.Queries;

public interface IArticleQueries
{
    public Task<List<ArticleListItemDto>> GetAllArticlesAsync(CancellationToken ct);

    public Task<ArticleDto?> GetArticlesByIdAsync(Guid id, CancellationToken ct);
}
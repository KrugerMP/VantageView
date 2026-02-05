namespace VantageView.API.Models;

public record ArticleDto(
    int Id,
    string Title,
    string Summary,
    string Content,
    string Author,
    DateTime PublishedAt);

public record ArticleListItemDto(
    int Id,
    string Title,
    string Summary,
    string Author,
    DateTime PublishedAt);

public record CreateArticleDto(
    string Title,
    string Summary,
    string Content,
    string Author,
    DateTime? PublishedAt);

public record UpdateArticleDto(
    string Title,
    string Summary,
    string Content,
    string Author,
    DateTime? PublishedAt);

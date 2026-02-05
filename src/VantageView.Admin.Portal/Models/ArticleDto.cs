using System.ComponentModel.DataAnnotations;

namespace VantageView.Admin.Portal.Models;

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

public class CreateArticleDto
{
    [Required]
    public string Title { get; set; } = "";
    [Required]
    public string Summary { get; set; } = "";
    [Required]
    public string Content { get; set; } = "";
    [Required]
    public string Author { get; set; } = "";
    public DateTime? PublishedAt { get; set; }
}

public class UpdateArticleDto
{
    [Required]
    public string Title { get; set; } = "";
    [Required]
    public string Summary { get; set; } = "";
    [Required]
    public string Content { get; set; } = "";
    [Required]
    public string Author { get; set; } = "";
    public DateTime? PublishedAt { get; set; }
}

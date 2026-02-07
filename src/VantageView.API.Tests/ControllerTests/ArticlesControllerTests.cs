using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VantageView.API.Controllers;
using VantageView.API.Domain.Articles.Queries;
using VantageView.API.Models;

namespace VantageView.API.Tests.ControllerTests;

/// <summary>
/// Unit tests for <see cref="ArticlesController"/> public article endpoints.
/// </summary>
[TestClass]
public class ArticlesControllerTests
{
    private readonly Mock<IArticleQueries> _articleQueriesMock;
    private readonly Mock<ILogger<ArticlesController>> _loggerMock;

    /// <summary>
    /// Initializes mocks for the articles controller and its dependencies.
    /// </summary>
    public ArticlesControllerTests()
    {
        _articleQueriesMock = new Mock<IArticleQueries>();
        _loggerMock = new Mock<ILogger<ArticlesController>>();
    }

    /// <summary>
    /// Verifies that GetArticlesAsync returns 200 with a list of articles when the query succeeds.
    /// </summary>
    [TestMethod]
    public async Task GetArticlesAsync_Returns200_WhenArticlesExistAsync()
    {
        // Arrange
        List<ArticleListItemDto> articles =
        [
            new(Guid.NewGuid(), "Title 1", "Summary 1", "Author 1", DateTime.UtcNow, DateTime.UtcNow)
        ];
        _articleQueriesMock.Setup(x => x.GetAllArticlesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(articles);

        ArticlesController controller = new(_articleQueriesMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new();

        // Act
        ObjectResult? result = await controller.GetArticlesAsync(cts.Token) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        BaseResponseModel<List<ArticleListItemDto>>? response = result.Value as BaseResponseModel<List<ArticleListItemDto>>;
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Result);
        Assert.AreEqual(1, response.Result.Count);
        Assert.AreEqual("Title 1", response.Result[0].Title);
        Assert.AreEqual("Articles retrieved successfully.", response.Message);
    }

    /// <summary>
    /// Verifies that GetArticlesAsync returns 200 with empty list when no articles exist.
    /// </summary>
    [TestMethod]
    public async Task GetArticlesAsync_Returns200_WhenNoArticlesExistAsync()
    {
        // Arrange
        _articleQueriesMock.Setup(x => x.GetAllArticlesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        ArticlesController controller = new(_articleQueriesMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new();

        // Act
        ObjectResult? result = await controller.GetArticlesAsync(cts.Token) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        BaseResponseModel<List<ArticleListItemDto>>? response = result.Value as BaseResponseModel<List<ArticleListItemDto>>;
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Result);
        Assert.AreEqual(0, response.Result.Count);
    }

    /// <summary>
    /// Verifies that GetArticlesAsync returns 400 when the query throws.
    /// </summary>
    [TestMethod]
    public async Task GetArticlesAsync_Returns400_WhenQueryThrowsAsync()
    {
        // Arrange
        _articleQueriesMock.Setup(x => x.GetAllArticlesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        ArticlesController controller = new(_articleQueriesMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new();

        // Act
        ObjectResult? result = await controller.GetArticlesAsync(cts.Token) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);

        BaseResponseModel<List<ArticleListItemDto>>? response = result.Value as BaseResponseModel<List<ArticleListItemDto>>;
        Assert.IsNotNull(response);
        Assert.AreEqual("Error fetching articles.", response.Message);
        Assert.IsNotNull(response.Error);
    }

    /// <summary>
    /// Verifies that GetArticleAsync returns 200 with article when found.
    /// </summary>
    [TestMethod]
    public async Task GetArticleAsync_Returns200_WhenArticleExistsAsync()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        ArticleDto articleDto = new(id, "Title", "Summary", "Content", "Author", now, now);
        _articleQueriesMock.Setup(x => x.GetArticlesByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(articleDto);

        ArticlesController controller = new(_articleQueriesMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new();

        // Act
        ObjectResult? result = await controller.GetArticleAsync(id, cts.Token) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Result);
        Assert.AreEqual(id, response.Result.Id);
        Assert.AreEqual("Title", response.Result.Title);
        Assert.AreEqual("Article retrieved successfully.", response.Message);
    }

    /// <summary>
    /// Verifies that GetArticleAsync returns 404 when article is not found.
    /// </summary>
    [TestMethod]
    public async Task GetArticleAsync_Returns404_WhenArticleNotFoundAsync()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _articleQueriesMock.Setup(x => x.GetArticlesByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArticleDto?)null);

        ArticlesController controller = new(_articleQueriesMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new();

        // Act
        NotFoundObjectResult? result = await controller.GetArticleAsync(id, cts.Token) as NotFoundObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);

        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual("Article not found.", response.Message);
    }

    /// <summary>
    /// Verifies that GetArticleAsync returns 400 when the query throws.
    /// </summary>
    [TestMethod]
    public async Task GetArticleAsync_Returns400_WhenQueryThrowsAsync()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        _articleQueriesMock.Setup(x => x.GetArticlesByIdAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        ArticlesController controller = new(_articleQueriesMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new();

        // Act
        ObjectResult? result = await controller.GetArticleAsync(id, cts.Token) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);

        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual("Error fetching article.", response.Message);
    }
}

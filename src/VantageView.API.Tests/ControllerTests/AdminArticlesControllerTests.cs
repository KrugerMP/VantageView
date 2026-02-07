using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VantageView.API.Controllers;
using VantageView.API.Domain.AdminArticles.Commands;
using VantageView.API.Domain.Articles.Queries;
using VantageView.API.Models;

namespace VantageView.API.Tests.ControllerTests;

/// <summary>
/// Unit tests for <see cref="AdminArticlesController"/> admin article endpoints.
/// </summary>
[TestClass]
public class AdminArticlesControllerTests
{
    private readonly Mock<ILogger<AdminArticlesController>> _loggerMock;
    private readonly Mock<IAdminArticlesCommands> _adminCommandsMock;
    private readonly Mock<IArticleQueries> _articleQueriesMock;

    /// <summary>
    /// Initializes mocks for the admin articles controller and its dependencies.
    /// </summary>
    public AdminArticlesControllerTests()
    {
        _loggerMock = new Mock<ILogger<AdminArticlesController>>();
        _adminCommandsMock = new Mock<IAdminArticlesCommands>();
        _articleQueriesMock = new Mock<IArticleQueries>();
    }

    /// <summary>
    /// Verifies that CreateArticleAsync returns 201 with the created article when the command succeeds.
    /// </summary>
    [TestMethod]
    public async Task CreateArticleAsync_Returns201_WhenCommandSucceedsAsync()
    {
        // Arrange
        ResetMocks();
        Guid id = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        CreateArticleDto createDto = new("Valid Title", "Summary", "Content", "Author", null);
        ArticleDto articleDto = new(id, "Valid Title", "Summary", "Content", "Author", now, now);
        _adminCommandsMock.Setup(x => x.CreateArticleAsync(It.IsAny<CreateArticleDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, articleDto));

        AdminArticlesController controller = new(_loggerMock.Object, _adminCommandsMock.Object, _articleQueriesMock.Object);
        CancellationTokenSource cts = new();

        // Act
        CreatedAtRouteResult? result = (await controller.CreateArticleAsync(createDto, cts.Token) as object) as CreatedAtRouteResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual("GetArticle", result.RouteName);
        Assert.IsNotNull(result.RouteValues);
        Assert.IsTrue(result.RouteValues!.ContainsKey("id"));
        Assert.AreEqual(id, result.RouteValues["id"]);

        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Result);
        Assert.AreEqual(id, response.Result.Id);
        Assert.AreEqual("Article created", response.Message);
    }

    /// <summary>
    /// Verifies that CreateArticleAsync returns 400 with validation errors when the DTO is invalid.
    /// </summary>
    [TestMethod]
    public async Task CreateArticleAsync_Returns400_WhenValidationFailsAsync()
    {
        // Arrange
        ResetMocks();
        CreateArticleDto invalidDto = new("", "Summary", "Content", "Author", null);

        AdminArticlesController controller = new(_loggerMock.Object, _adminCommandsMock.Object, _articleQueriesMock.Object);
        CancellationTokenSource cts = new();

        // Act
        BadRequestObjectResult? result = (await controller.CreateArticleAsync(invalidDto, cts.Token) as object) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        ValidationErrorResponse? response = result.Value as ValidationErrorResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("One or more validation errors occurred.", response.Message);
        Assert.IsTrue(response.Errors.Count > 0);
    }

    /// <summary>
    /// Verifies that CreateArticleAsync returns 400 when the command returns failure.
    /// </summary>
    [TestMethod]
    public async Task CreateArticleAsync_Returns400_WhenCommandFailsAsync()
    {
        // Arrange
        ResetMocks();
        CreateArticleDto createDto = new("Valid Title", "Summary", "Content", "Author", null);
        _adminCommandsMock.Setup(x => x.CreateArticleAsync(It.IsAny<CreateArticleDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, (ArticleDto?)null));

        AdminArticlesController controller = new(_loggerMock.Object, _adminCommandsMock.Object, _articleQueriesMock.Object);
        CancellationTokenSource cts = new();

        // Act
        BadRequestObjectResult? result = (await controller.CreateArticleAsync(createDto, cts.Token) as object) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual("An error occurred when creating the article", response.Message);
    }

    /// <summary>
    /// Verifies that CreateArticleAsync returns 400 when the command throws.
    /// </summary>
    [TestMethod]
    public async Task CreateArticleAsync_Returns400_WhenCommandThrowsAsync()
    {
        // Arrange
        ResetMocks();
        CreateArticleDto createDto = new("Valid Title", "Summary", "Content", "Author", null);
        _adminCommandsMock.Setup(x => x.CreateArticleAsync(It.IsAny<CreateArticleDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        AdminArticlesController controller = new(_loggerMock.Object, _adminCommandsMock.Object, _articleQueriesMock.Object);
        CancellationTokenSource cts = new();

        // Act
        BadRequestObjectResult? result = (await controller.CreateArticleAsync(createDto, cts.Token) as object) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual("An error occurred when creating the article", response.Message);
    }

    /// <summary>
    /// Verifies that UpdateArticleAsync returns 200 with the updated article when the article exists and command succeeds.
    /// </summary>
    [TestMethod]
    public async Task UpdateArticleAsync_Returns200_WhenArticleExistsAndCommandSucceedsAsync()
    {
        // Arrange
        ResetMocks();
        Guid id = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        ArticleDto existingArticle = new(id, "Old Title", "Old Summary", "Old Content", "Author", now, now);
        ArticleDto updatedArticle = new(id, "New Title", "New Summary", "New Content", "Author", now, now);
        UpdateArticleDto updateDto = new("New Title", "New Summary", "New Content");

        _articleQueriesMock.Setup(x => x.GetArticlesByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingArticle);
        _adminCommandsMock.Setup(x => x.UpdateArticleAsync(id, It.IsAny<UpdateArticleDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, updatedArticle));

        AdminArticlesController controller = new(_loggerMock.Object, _adminCommandsMock.Object, _articleQueriesMock.Object);
        CancellationTokenSource cts = new();

        // Act
        ObjectResult? result = (await controller.UpdateArticleAsync(id, updateDto, cts.Token) as object) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Result);
        Assert.AreEqual("New Title", response.Result.Title);
        Assert.AreEqual("Article updated", response.Message);
    }

    /// <summary>
    /// Verifies that UpdateArticleAsync returns 404 when the article is not found by id before update.
    /// </summary>
    [TestMethod]
    public async Task UpdateArticleAsync_Returns404_WhenArticleNotFoundAsync()
    {
        // Arrange
        ResetMocks();
        Guid id = Guid.NewGuid();
        UpdateArticleDto updateDto = new("Title", "Summary", "Content");
        _articleQueriesMock.Setup(x => x.GetArticlesByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ArticleDto?)null);

        AdminArticlesController controller = new(_loggerMock.Object, _adminCommandsMock.Object, _articleQueriesMock.Object);
        CancellationTokenSource cts = new();

        // Act
        NotFoundObjectResult? result = (await controller.UpdateArticleAsync(id, updateDto, cts.Token) as object) as NotFoundObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual("Error processing request", response.Message);
    }

    /// <summary>
    /// Verifies that UpdateArticleAsync returns 404 when the command returns not found.
    /// </summary>
    [TestMethod]
    public async Task UpdateArticleAsync_Returns404_WhenCommandReturnsNotFoundAsync()
    {
        // Arrange
        ResetMocks();
        Guid id = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        ArticleDto existingArticle = new(id, "Title", "Summary", "Content", "Author", now, now);
        UpdateArticleDto updateDto = new("Title", "Summary", "Content");

        _articleQueriesMock.Setup(x => x.GetArticlesByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingArticle);
        _adminCommandsMock.Setup(x => x.UpdateArticleAsync(id, It.IsAny<UpdateArticleDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, (ArticleDto?)null));

        AdminArticlesController controller = new(_loggerMock.Object, _adminCommandsMock.Object, _articleQueriesMock.Object);
        CancellationTokenSource cts = new();

        // Act
        NotFoundObjectResult? result = (await controller.UpdateArticleAsync(id, updateDto, cts.Token) as object) as NotFoundObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual("Error processing request", response.Message);
    }

    /// <summary>
    /// Verifies that UpdateArticleAsync returns 400 when the DTO fails validation.
    /// </summary>
    [TestMethod]
    public async Task UpdateArticleAsync_Returns400_WhenValidationFailsAsync()
    {
        // Arrange
        ResetMocks();
        Guid id = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        ArticleDto existingArticle = new(id, "Title", "Summary", "Content", "Author", now, now);
        UpdateArticleDto invalidUpdateDto = new("", "Summary", "Content");

        _articleQueriesMock.Setup(x => x.GetArticlesByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingArticle);

        AdminArticlesController controller = new(_loggerMock.Object, _adminCommandsMock.Object, _articleQueriesMock.Object);
        CancellationTokenSource cts = new();

        // Act
        BadRequestObjectResult? result = (await controller.UpdateArticleAsync(id, invalidUpdateDto, cts.Token) as object) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        ValidationErrorResponse? response = result.Value as ValidationErrorResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual("One or more validation errors occurred.", response.Message);
    }

    /// <summary>
    /// Verifies that DeleteArticleAsync returns 200 with the deleted article when the article exists.
    /// </summary>
    [TestMethod]
    public async Task DeleteArticleAsync_Returns200_WhenArticleExistsAsync()
    {
        // Arrange
        ResetMocks();
        Guid id = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        ArticleDto articleDto = new(id, "Title", "Summary", "Content", "Author", now, now);
        _adminCommandsMock.Setup(x => x.DeleteArticleAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((true, articleDto));

        AdminArticlesController controller = new(_loggerMock.Object, _adminCommandsMock.Object, _articleQueriesMock.Object);
        CancellationTokenSource cts = new();

        // Act
        ObjectResult? result = (await controller.DeleteArticleAsync(id, cts.Token) as object) as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Result);
        Assert.AreEqual(id, response.Result.Id);
        Assert.AreEqual("Article deleted", response.Message);
    }

    /// <summary>
    /// Verifies that DeleteArticleAsync returns 404 when the article is not found.
    /// </summary>
    [TestMethod]
    public async Task DeleteArticleAsync_Returns404_WhenArticleNotFoundAsync()
    {
        // Arrange
        ResetMocks();
        Guid id = Guid.NewGuid();
        _adminCommandsMock.Setup(x => x.DeleteArticleAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((false, (ArticleDto?)null));

        AdminArticlesController controller = new(_loggerMock.Object, _adminCommandsMock.Object, _articleQueriesMock.Object);
        CancellationTokenSource cts = new();

        // Act
        NotFoundObjectResult? result = (await controller.DeleteArticleAsync(id, cts.Token) as object) as NotFoundObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(404, result.StatusCode);
        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual("Error processing request", response.Message);
    }

    /// <summary>
    /// Verifies that DeleteArticleAsync returns 400 when the command throws.
    /// </summary>
    [TestMethod]
    public async Task DeleteArticleAsync_Returns400_WhenCommandThrowsAsync()
    {
        // Arrange
        ResetMocks();
        Guid id = Guid.NewGuid();
        _adminCommandsMock.Setup(x => x.DeleteArticleAsync(id, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        AdminArticlesController controller = new(_loggerMock.Object, _adminCommandsMock.Object, _articleQueriesMock.Object);
        CancellationTokenSource cts = new();

        // Act
        BadRequestObjectResult? result = (await controller.DeleteArticleAsync(id, cts.Token) as object) as BadRequestObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
        BaseResponseModel<ArticleDto>? response = result.Value as BaseResponseModel<ArticleDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual("Error deleting article.", response.Message);
    }

    /// <summary>
    /// Resets all mocks to a clean state.
    /// </summary>
    private void ResetMocks()
    {
        _adminCommandsMock.Reset();
        _articleQueriesMock.Reset();
        _loggerMock.Reset();
    }
}

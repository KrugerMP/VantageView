using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VantageView.API.Controllers;
using VantageView.API.Domain.Health.Queries;
using VantageView.API.Models;

namespace VantageView.API.Tests.ControllerTests;

/// <summary>
/// Unit tests for <see cref="HealthController"/> health check endpoints.
/// </summary>
[TestClass]
public class HealthControllerTests
{
    private readonly Mock<ILogger<HealthController>> _loggerMock;
    private readonly Mock<IHealthQueries> _healthMock;

    /// <summary>
    /// Initializes mocks for the health controller and its dependencies.
    /// </summary>
    public HealthControllerTests()
    {
        _loggerMock = new Mock<ILogger<HealthController>>();
        _healthMock = new Mock<IHealthQueries>();
    }

    /// <summary>
    /// Verifies that the articles health endpoint returns 400 when the service is unhealthy (no articles).
    /// </summary>
    [TestMethod]
    public async Task HealthController_GetArticlesServiceHealthAsync_Negative_TestAsync()
    {
        // Arrange
        ResetMocks();

        HealthController healthController = new(_healthMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new();

        // Act
        ObjectResult? result = await healthController.GetArticlesServiceHealthAsync(cts.Token) as ObjectResult;

        // Assert: 400 when no articles exist
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);

        BaseResponseModel<HealthDto>? response = result.Value as BaseResponseModel<HealthDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual("Health of service could not be guaranteed", response.Message);
        Assert.IsNotNull(response.Result);
        Assert.IsFalse(response.Result.IsHealthy);
    }

    /// <summary>
    /// Verifies that the articles health endpoint returns 200 when the service is healthy.
    /// </summary>
    [TestMethod]
    public async Task HealthController_GetArticlesServiceHealthAsync_Positive_TestAsync()
    {
        // Arrange
        ResetMocks();

        _healthMock.Setup(x => x.IsHealthyAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

        HealthController healthController = new(_healthMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new();

        // Act
        ObjectResult? result = await healthController.GetArticlesServiceHealthAsync(cts.Token) as ObjectResult;

        // Assert: 400 when no articles exist
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        BaseResponseModel<HealthDto>? response = result.Value as BaseResponseModel<HealthDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual("Successfully processed health request", response.Message);
        Assert.IsNotNull(response.Result);
        Assert.IsTrue(response.Result.IsHealthy);
    }

    /// <summary>
    /// Verifies that the articles health endpoint returns 400 when the health query throws an exception.
    /// </summary>
    [TestMethod]
    public async Task HealthController_GetArticlesServiceHealthAsync_Exception_TestAsync()
    {
        // Arrange
        ResetMocks();

        _healthMock.Setup(x => x.IsHealthyAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception("This is unit testing"));

        HealthController healthController = new(_healthMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new();

        // Act
        ObjectResult? result = await healthController.GetArticlesServiceHealthAsync(cts.Token) as ObjectResult;

        // Assert: 400 when no articles exist
        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);

        BaseResponseModel<HealthDto>? response = result.Value as BaseResponseModel<HealthDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual("Error checking articles service health.", response.Message);
        Assert.IsNotNull(response.Result);
        Assert.IsFalse(response.Result.IsHealthy);
    }

    /// <summary>
    /// Attempts to clear all mocks
    /// </summary>
    private void ResetMocks()
    {
        _healthMock.Reset();
        _loggerMock.Reset();
    }
}
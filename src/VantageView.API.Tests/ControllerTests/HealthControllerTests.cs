using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using VantageView.API.Controllers;
using VantageView.API.Domain.Health.Queries;
using VantageView.API.Models;

namespace VantageView.API.Tests.ControllerTests;

[TestClass]
public class HealthControllerTests
{
    private readonly Mock<ILogger<HealthController>> _loggerMock;
    private readonly Mock<IHealthQueries> _healthMock;

    public HealthControllerTests()
    {
        _loggerMock = new Mock<ILogger<HealthController>>();
        _healthMock = new Mock<IHealthQueries>();
    }


    [TestMethod]
    public async Task HealthController_GetArticlesServiceHealthAsync_Negative_Test()
    {
        // Arrange
        ResetMocks();

        HealthController healthController = new HealthController(_healthMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new CancellationTokenSource();

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

    [TestMethod]
    public async Task HealthController_GetArticlesServiceHealthAsync_Positive_Test()
    {
        // Arrange
        ResetMocks();

        _healthMock.Setup(x => x.IsHealthyAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

        HealthController healthController = new HealthController(_healthMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new CancellationTokenSource();

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

    [TestMethod]
    public async Task HealthController_GetArticlesServiceHealthAsync_Exception_Test()
    {
        // Arrange
        ResetMocks();

        _healthMock.Setup(x => x.IsHealthyAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception("This is unit testing"));

        HealthController healthController = new HealthController(_healthMock.Object, _loggerMock.Object);
        CancellationTokenSource cts = new CancellationTokenSource();

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
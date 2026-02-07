using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using VantageView.API.Controllers;
using VantageView.API.Models;
using VantageView.Data;

namespace VantageView.API.Tests.ControllerTests;

[TestClass]
public class HealthControllerTests
{
    private readonly Mock<ILogger<HealthController>> _loggerMock;

    public HealthControllerTests()
    {
        _loggerMock = new Mock<ILogger<HealthController>>();
    }

    private static AppDbContext CreateInMemoryContext()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;
        
        AppDbContext context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        return context;
    }

    [TestMethod]
    public async Task HealthController_GetArticlesServiceHealthAsync_Negative_Test()
    {
        // Arrange: in-memory DB with no articles (unhealthy)
        await using AppDbContext db = CreateInMemoryContext();
        
        HealthController healthController = new HealthController(db, _loggerMock.Object);
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
}
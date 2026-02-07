using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VantageView.Auth.Domain.Auth.Commands;
using VantageView.Auth.Models;

namespace VantageView.Auth.Controllers;

/// <summary>
/// Authentication endpoints for admin login and JWT issuance.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthCommands _authCommands;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authCommands">The auth command service (e.g. login, token issuance).</param>
    /// <param name="logger">The logger for auth diagnostics.</param>
    public AuthController(IAuthCommands authCommands, ILogger<AuthController> logger)
    {
        _authCommands = authCommands;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates an admin user and returns a JWT bearer token.
    /// </summary>
    /// <param name="request">Login credentials.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>200 OK with JWT token on success; 401 Unauthorized on invalid credentials; 500 on error.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(BaseResponseModel<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LoginAsync([FromBody, Required] LoginRequest request, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("User [{Username}] logged in at {Time}", request.Username, DateTime.UtcNow);
            (bool logonResult, string message, string token) = await _authCommands.LogonUserAsync(request, ct);

            if (!logonResult)
            {
                return Unauthorized(new BaseResponseModel<LoginResponse>
                {
                    Message = "Invalid credentials.",
                    ResponseTime = DateTime.UtcNow,
                    Error = new ErrorResponseModel { Message = message },
                    Result = null!
                });
            }

            return Ok(new BaseResponseModel<LoginResponse>
            {
                Result = new LoginResponse(token),
                Message = "Login successful.",
                ResponseTime = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for user {Username}", request.Username);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseModel<LoginResponse>
            {
                Message = "An error occurred while processing your request.",
                ResponseTime = DateTime.UtcNow,
                Error = new ErrorResponseModel { Message = "An error occurred while processing your request." },
                Result = null!
            });
        }
    }
}

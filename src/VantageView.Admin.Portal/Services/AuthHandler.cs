using System.Net.Http.Headers;

namespace VantageView.Admin.Portal.Services;

/// <summary>
/// HTTP message handler that adds the JWT Bearer token to outbound API requests.
/// </summary>
/// <param name="authService">The auth service providing the current token.</param>
public class AuthHandler(AuthService authService) : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (authService.Token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authService.Token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}

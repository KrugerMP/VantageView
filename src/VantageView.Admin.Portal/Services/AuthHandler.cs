using System.Net.Http.Headers;

namespace VantageView.Admin.Portal.Services;

public class AuthHandler(AuthService authService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (authService.Token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authService.Token);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}

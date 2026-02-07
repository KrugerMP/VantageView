
using VantageView.Auth.Models;

namespace VantageView.Auth.Domain.Auth.Commands;

public interface IAuthCommands
{
    public Task<(bool logonResult, string message, string token)> LogonUserAsync(LoginRequest loginRequest, CancellationToken ct);
}
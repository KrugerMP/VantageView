namespace VantageView.Admin.Portal.Services;

public class AuthService
{
    public string? Token { get; private set; }
    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

    public void SetToken(string token) => Token = token;
    public void ClearToken() => Token = null;
}

using System.ComponentModel.DataAnnotations;

namespace VantageView.Admin.Portal.Models;

public class LoginRequest
{
    [Required]
    public string Username { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
}

public record LoginResponse(string Token);

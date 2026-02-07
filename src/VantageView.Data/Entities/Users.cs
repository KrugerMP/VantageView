namespace VantageView.Data.Entities;

/// <summary>
/// Represents a user in the system (e.g. admin portal authentication).
/// </summary>
public class Users
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the login name for the user.
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// Gets or sets the user's password (stored hash in production).
    /// </summary>
    public required string Password { get; set; }
}
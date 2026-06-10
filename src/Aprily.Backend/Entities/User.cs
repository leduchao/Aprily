namespace Aprily.Backend.Entities;

public class User : AuditableEntity
{
    public string? FullName { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public bool IsEmailVerified { get; set; }

    public DateTime LastLoginAt { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

}

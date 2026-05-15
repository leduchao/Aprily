using Aprily.SharedKernel;

namespace Aprily.Domain.Entities;

public class User : Entity
{
    public string? Username { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? AvatarUrl { get; set; }

    public DateTime LastLoginAt { get; set; }

    public ICollection<Thread> Threads { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
}

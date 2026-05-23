using Aprily.SharedKernel;

namespace Aprily.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public string Token { get; set; } = null!;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public string? ReplacedByToken { get; set; }
}

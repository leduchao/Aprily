using System;
using System.Collections.Generic;

namespace Aprily.Backend.Entities;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public bool IsRevoked { get; set; }

    public DateTime ExpiresAt { get; set; }

    public string? ReplacedByToken { get; set; }

    public Guid EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User User { get; set; } = null!;
}

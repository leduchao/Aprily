using System;
using System.Collections.Generic;

namespace Aprily.Backend.Entities;

public partial class MessageReaction
{
    public int Id { get; set; }

    public int MessageId { get; set; }

    public int UserId { get; set; }

    public string Type { get; set; } = null!;

    public Guid EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Message Message { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

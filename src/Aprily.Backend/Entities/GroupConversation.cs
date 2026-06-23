using System;
using System.Collections.Generic;

namespace Aprily.Backend.Entities;

public partial class GroupConversation
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public string Name { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public int CreatedByUserId { get; set; }

    public Guid EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User CreatedByUser { get; set; } = null!;
}

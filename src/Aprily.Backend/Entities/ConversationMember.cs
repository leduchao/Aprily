using System;
using System.Collections.Generic;

namespace Aprily.Backend.Entities;

public partial class ConversationMember
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int UserId { get; set; }

    public int? LastReadMessageId { get; set; }

    public DateTime? LastReadAt { get; set; }

    public Guid EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual Message? LastReadMessage { get; set; }

    public virtual User User { get; set; } = null!;
}

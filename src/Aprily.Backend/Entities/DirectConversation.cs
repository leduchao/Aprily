using System;
using System.Collections.Generic;

namespace Aprily.Backend.Entities;

public partial class DirectConversation
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int UserLowId { get; set; }

    public int UserHighId { get; set; }

    public Guid EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User UserHigh { get; set; } = null!;

    public virtual User UserLow { get; set; } = null!;
}

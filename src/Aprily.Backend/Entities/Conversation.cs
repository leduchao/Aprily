using System;
using System.Collections.Generic;

namespace Aprily.Backend.Entities;

public partial class Conversation
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public int? LastMessageId { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public Guid EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();

    public virtual DirectConversation? DirectConversation { get; set; }

    public virtual Message? LastMessage { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}

using System;
using System.Collections.Generic;

namespace Aprily.Backend.Entities;

public partial class Message
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public int SenderUserId { get; set; }

    public int? ReplyToMessageId { get; set; }

    public string? Content { get; set; }

    public DateTime SentAt { get; set; }

    public Guid EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();

    public virtual ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public virtual ICollection<Message> InverseReplyToMessage { get; set; } = new List<Message>();

    public virtual ICollection<MessageAttachment> MessageAttachments { get; set; } = new List<MessageAttachment>();

    public virtual ICollection<MessageReaction> MessageReactions { get; set; } = new List<MessageReaction>();

    public virtual Message? ReplyToMessage { get; set; }

    public virtual User SenderUser { get; set; } = null!;
}

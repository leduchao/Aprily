using System;
using System.Collections.Generic;

namespace Aprily.Backend.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; }

    public string? AvatarUrl { get; set; }

    public bool IsEmailVerified { get; set; }

    public DateTime LastSignInAt { get; set; }

    public Guid EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ConversationMember> ConversationMembers { get; set; } = new List<ConversationMember>();

    public virtual ICollection<DirectConversation> DirectConversationUserHighs { get; set; } = new List<DirectConversation>();

    public virtual ICollection<DirectConversation> DirectConversationUserLows { get; set; } = new List<DirectConversation>();

    public virtual ICollection<FriendRequest> FriendRequestAddresseeUsers { get; set; } = new List<FriendRequest>();

    public virtual ICollection<FriendRequest> FriendRequestRequesterUsers { get; set; } = new List<FriendRequest>();

    public virtual ICollection<Friendship> FriendshipUserHighs { get; set; } = new List<Friendship>();

    public virtual ICollection<Friendship> FriendshipUserLows { get; set; } = new List<Friendship>();

    public virtual ICollection<MessageReaction> MessageReactions { get; set; } = new List<MessageReaction>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

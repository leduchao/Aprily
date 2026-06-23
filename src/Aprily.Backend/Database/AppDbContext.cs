using System;
using System.Collections.Generic;
using Aprily.Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Database;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<ConversationMember> ConversationMembers { get; set; }

    public virtual DbSet<DirectConversation> DirectConversations { get; set; }

    public virtual DbSet<FriendRequest> FriendRequests { get; set; }

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageAttachment> MessageAttachments { get; set; }

    public virtual DbSet<MessageReaction> MessageReactions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("conversations_pkey");

            entity.ToTable("conversations");

            entity.HasIndex(e => e.LastMessageAt, "ix_conversations_last_message_at").IsDescending();

            entity.HasIndex(e => e.EntityId, "ux_conversations_entity_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("entity_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.LastMessageAt).HasColumnName("last_message_at");
            entity.Property(e => e.LastMessageId).HasColumnName("last_message_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasDefaultValueSql("'direct'::character varying")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.LastMessage).WithMany(p => p.Conversations)
                .HasForeignKey(d => d.LastMessageId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_conversations_messages_last_message_id");
        });

        modelBuilder.Entity<ConversationMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("conversation_members_pkey");

            entity.ToTable("conversation_members");

            entity.HasIndex(e => e.ConversationId, "ix_conversation_members_conversation_id");

            entity.HasIndex(e => e.UserId, "ix_conversation_members_user_id");

            entity.HasIndex(e => new { e.ConversationId, e.UserId }, "ux_conversation_members_conversation_id_user_id")
                .IsUnique()
                .HasFilter("(is_deleted = false)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("entity_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.LastReadAt).HasColumnName("last_read_at");
            entity.Property(e => e.LastReadMessageId).HasColumnName("last_read_message_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Conversation).WithMany(p => p.ConversationMembers)
                .HasForeignKey(d => d.ConversationId)
                .HasConstraintName("fk_conversation_members_conversations_conversation_id");

            entity.HasOne(d => d.LastReadMessage).WithMany(p => p.ConversationMembers)
                .HasForeignKey(d => d.LastReadMessageId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_conversation_members_messages_last_read_message_id");

            entity.HasOne(d => d.User).WithMany(p => p.ConversationMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_conversation_members_users_user_id");
        });

        modelBuilder.Entity<DirectConversation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("direct_conversations_pkey");

            entity.ToTable("direct_conversations");

            entity.HasIndex(e => e.ConversationId, "ux_direct_conversations_conversation_id")
                .IsUnique()
                .HasFilter("(is_deleted = false)");

            entity.HasIndex(e => new { e.UserLowId, e.UserHighId }, "ux_direct_conversations_user_pair")
                .IsUnique()
                .HasFilter("(is_deleted = false)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("entity_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserHighId).HasColumnName("user_high_id");
            entity.Property(e => e.UserLowId).HasColumnName("user_low_id");

            entity.HasOne(d => d.Conversation).WithOne(p => p.DirectConversation)
                .HasForeignKey<DirectConversation>(d => d.ConversationId)
                .HasConstraintName("fk_direct_conversations_conversations_conversation_id");

            entity.HasOne(d => d.UserHigh).WithMany(p => p.DirectConversationUserHighs)
                .HasForeignKey(d => d.UserHighId)
                .HasConstraintName("fk_direct_conversations_users_user_high_id");

            entity.HasOne(d => d.UserLow).WithMany(p => p.DirectConversationUserLows)
                .HasForeignKey(d => d.UserLowId)
                .HasConstraintName("fk_direct_conversations_users_user_low_id");
        });

        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("friend_requests_pkey");

            entity.ToTable("friend_requests");

            entity.HasIndex(e => e.AddresseeUserId, "ix_friend_requests_addressee_user_id");

            entity.HasIndex(e => e.RequesterUserId, "ix_friend_requests_requester_user_id");

            entity.HasIndex(e => e.EntityId, "ux_friend_requests_entity_id").IsUnique();

            entity.HasIndex(e => new { e.UserLowId, e.UserHighId }, "ux_friend_requests_pending_user_pair")
                .IsUnique()
                .HasFilter("(((status)::text = 'pending'::text) AND (is_deleted = false))");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddresseeUserId).HasColumnName("addressee_user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("entity_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.RequesterUserId).HasColumnName("requester_user_id");
            entity.Property(e => e.RespondedAt).HasColumnName("responded_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserHighId)
                .HasComputedColumnSql("GREATEST(requester_user_id, addressee_user_id)", true)
                .HasColumnName("user_high_id");
            entity.Property(e => e.UserLowId)
                .HasComputedColumnSql("LEAST(requester_user_id, addressee_user_id)", true)
                .HasColumnName("user_low_id");

            entity.HasOne(d => d.AddresseeUser).WithMany(p => p.FriendRequestAddresseeUsers)
                .HasForeignKey(d => d.AddresseeUserId)
                .HasConstraintName("fk_friend_requests_users_addressee_user_id");

            entity.HasOne(d => d.RequesterUser).WithMany(p => p.FriendRequestRequesterUsers)
                .HasForeignKey(d => d.RequesterUserId)
                .HasConstraintName("fk_friend_requests_users_requester_user_id");
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("friendships_pkey");

            entity.ToTable("friendships");

            entity.HasIndex(e => e.UserHighId, "ix_friendships_user_high_id");

            entity.HasIndex(e => e.UserLowId, "ix_friendships_user_low_id");

            entity.HasIndex(e => e.EntityId, "ux_friendships_entity_id").IsUnique();

            entity.HasIndex(e => new { e.UserLowId, e.UserHighId }, "ux_friendships_user_pair")
                .IsUnique()
                .HasFilter("(is_deleted = false)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcceptedRequestId).HasColumnName("accepted_request_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("entity_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserHighId).HasColumnName("user_high_id");
            entity.Property(e => e.UserLowId).HasColumnName("user_low_id");

            entity.HasOne(d => d.AcceptedRequest).WithMany(p => p.Friendships)
                .HasForeignKey(d => d.AcceptedRequestId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_friendships_friend_requests_accepted_request_id");

            entity.HasOne(d => d.UserHigh).WithMany(p => p.FriendshipUserHighs)
                .HasForeignKey(d => d.UserHighId)
                .HasConstraintName("fk_friendships_users_user_high_id");

            entity.HasOne(d => d.UserLow).WithMany(p => p.FriendshipUserLows)
                .HasForeignKey(d => d.UserLowId)
                .HasConstraintName("fk_friendships_users_user_low_id");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("messages_pkey");

            entity.ToTable("messages");

            entity.HasIndex(e => new { e.ConversationId, e.SentAt, e.Id }, "ix_messages_conversation_id_sent_at_id").IsDescending(false, true, true);

            entity.HasIndex(e => e.ReplyToMessageId, "ix_messages_reply_to_message_id");

            entity.HasIndex(e => e.SenderUserId, "ix_messages_sender_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(4000)
                .HasColumnName("content");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("entity_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ReplyToMessageId).HasColumnName("reply_to_message_id");
            entity.Property(e => e.SenderUserId).HasColumnName("sender_user_id");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("sent_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .HasConstraintName("fk_messages_conversations_conversation_id");

            entity.HasOne(d => d.ReplyToMessage).WithMany(p => p.InverseReplyToMessage)
                .HasForeignKey(d => d.ReplyToMessageId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_messages_messages_reply_to_message_id");

            entity.HasOne(d => d.SenderUser).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderUserId)
                .HasConstraintName("fk_messages_users_sender_user_id");
        });

        modelBuilder.Entity<MessageAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("message_attachments_pkey");

            entity.ToTable("message_attachments");

            entity.HasIndex(e => e.MessageId, "ix_message_attachments_message_id");

            entity.HasIndex(e => e.EntityId, "ux_message_attachments_entity_id").IsUnique();

            entity.HasIndex(e => new { e.MessageId, e.SortOrder }, "ux_message_attachments_message_id_sort_order")
                .IsUnique()
                .HasFilter("(is_deleted = false)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ContentType)
                .HasMaxLength(100)
                .HasColumnName("content_type");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("entity_id");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.OriginalFileName)
                .HasMaxLength(255)
                .HasColumnName("original_file_name");
            entity.Property(e => e.SizeBytes).HasColumnName("size_bytes");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.Url)
                .HasMaxLength(2048)
                .HasColumnName("url");
            entity.Property(e => e.Width).HasColumnName("width");

            entity.HasOne(d => d.Message).WithMany(p => p.MessageAttachments)
                .HasForeignKey(d => d.MessageId)
                .HasConstraintName("fk_message_attachments_messages_message_id");
        });

        modelBuilder.Entity<MessageReaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("message_reactions_pkey");

            entity.ToTable("message_reactions");

            entity.HasIndex(e => new { e.MessageId, e.Type }, "ix_message_reactions_message_id_type").HasFilter("(is_deleted = false)");

            entity.HasIndex(e => e.UserId, "ix_message_reactions_user_id");

            entity.HasIndex(e => e.EntityId, "ux_message_reactions_entity_id").IsUnique();

            entity.HasIndex(e => new { e.MessageId, e.UserId }, "ux_message_reactions_message_id_user_id")
                .IsUnique()
                .HasFilter("(is_deleted = false)");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("entity_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Message).WithMany(p => p.MessageReactions)
                .HasForeignKey(d => d.MessageId)
                .HasConstraintName("fk_message_reactions_messages_message_id");

            entity.HasOne(d => d.User).WithMany(p => p.MessageReactions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_message_reactions_users_user_id");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refresh_tokens_pkey");

            entity.ToTable("refresh_tokens");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.EntityId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("entity_id");
            entity.Property(e => e.ExpiresAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("expires_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsRevoked).HasColumnName("is_revoked");
            entity.Property(e => e.ReplacedByToken)
                .HasColumnType("character varying")
                .HasColumnName("replaced_by_token");
            entity.Property(e => e.Token)
                .HasColumnType("character varying")
                .HasColumnName("token");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_refresh_token_users_user_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvatarUrl)
                .HasColumnType("character varying")
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.EntityId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("entity_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsEmailVerified).HasColumnName("is_email_verified");
            entity.Property(e => e.LastSignInAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("last_sign_in_at");
            entity.Property(e => e.PasswordHash)
                .HasColumnType("character varying")
                .HasColumnName("password_hash");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(20)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

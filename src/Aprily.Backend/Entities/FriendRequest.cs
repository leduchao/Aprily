using System;
using System.Collections.Generic;

namespace Aprily.Backend.Entities;

public partial class FriendRequest
{
    public int Id { get; set; }

    public int RequesterUserId { get; set; }

    public int AddresseeUserId { get; set; }

    public int? UserLowId { get; set; }

    public int? UserHighId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? RespondedAt { get; set; }

    public Guid EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User AddresseeUser { get; set; } = null!;

    public virtual ICollection<Friendship> Friendships { get; set; } = new List<Friendship>();

    public virtual User RequesterUser { get; set; } = null!;
}

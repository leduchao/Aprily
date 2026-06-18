using System;
using System.Collections.Generic;

namespace Aprily.Backend.Entities;

public partial class Friendship
{
    public int Id { get; set; }

    public int UserLowId { get; set; }

    public int UserHighId { get; set; }

    public int? AcceptedRequestId { get; set; }

    public Guid EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual FriendRequest? AcceptedRequest { get; set; }

    public virtual User UserHigh { get; set; } = null!;

    public virtual User UserLow { get; set; } = null!;
}

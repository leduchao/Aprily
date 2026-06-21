using System;
using System.Collections.Generic;

namespace Aprily.Backend.Entities;

public partial class MessageAttachment
{
    public int Id { get; set; }

    public int MessageId { get; set; }

    public string Type { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string? OriginalFileName { get; set; }

    public string ContentType { get; set; } = null!;

    public long SizeBytes { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public short SortOrder { get; set; }

    public Guid EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Message Message { get; set; } = null!;
}

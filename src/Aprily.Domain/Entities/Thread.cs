using Aprily.SharedKernel;

namespace Aprily.Domain.Entities;

public class Thread : BaseEntity
{
    public int Type { get; set; } // 0: direct, 1: group
    public string? DirectConversationKey { get; set; }

    public int CreatorId { get; set; }
    public User? Creator { get; set; }

    public ICollection<ThreadParticipant> Participants { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
}

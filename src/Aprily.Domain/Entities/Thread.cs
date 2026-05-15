using Aprily.SharedKernel;

namespace Aprily.Domain.Entities;

public class Thread : Entity
{
    public int Type { get; set; } // 0: person2person, 1: group...

    public int CreatorId { get; set; }
    public User Creator { get; set; } = null!;

    public ICollection<Message> Messages { get; set; } = [];
}

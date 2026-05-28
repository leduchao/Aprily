using Aprily.SharedKernel;

namespace Aprily.Domain.Entities;

public class Message : BaseEntity
{
    public string Content { get; set; } = null!;

    public int SenderId { get; set; }
    public User? Sender { get; set; }

    public int ThreadId { get; set; }
    public Thread? Thread { get; set; }
}

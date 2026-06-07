namespace Aprily.Domain.Entities;

public class ThreadParticipant
{
    public int ThreadId { get; set; }
    public Thread? Thread { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

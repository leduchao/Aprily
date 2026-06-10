namespace Aprily.Backend.Features.Users.Services.Abstractions;

public interface ICurrentUser
{
    public Guid UserEntityId { get; }
}

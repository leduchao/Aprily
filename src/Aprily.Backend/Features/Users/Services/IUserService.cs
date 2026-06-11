using Aprily.Backend.Entities;

namespace Aprily.Backend.Features.Users.Services;

public interface IUserService
{
    Task<User?> CheckExistedEmailAsync(string email, CancellationToken cancellationToken = default);
}

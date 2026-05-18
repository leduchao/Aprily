using Aprily.Domain.Entities;

namespace Aprily.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByEmail(string email);
    Task<User> AddUser(User user, CancellationToken ct);
    Task<int> UpdateUser(User user, CancellationToken ct);
}
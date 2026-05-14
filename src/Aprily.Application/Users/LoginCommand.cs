using System.Text;

using Aprily.Application.Abstractions.Cqrs;
using Aprily.SharedKernel;

namespace Aprily.Application.Users;

public record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;

public record LoginResponse(string AccessToken);

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken ct)
    {
        var combine = command.Email + command.Password + Guid.NewGuid();
        var bytes = Encoding.UTF8.GetBytes(combine);
        string accessToken = Convert.ToBase64String(bytes);

        await Task.CompletedTask;

        return Result<LoginResponse>.Success(new LoginResponse(accessToken));
    }
}

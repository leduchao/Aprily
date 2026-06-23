using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Users.Models;
using Aprily.Backend.Features.Users.Services;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Users.UseCases.UpdateCurrentUserProfile;

public sealed record UpdateCurrentUserProfileCommand(string? FullName) : IRequest<Result<UserBasicInfo>>;

public sealed class UpdateCurrentUserProfileCommandValidator : AbstractValidator<UpdateCurrentUserProfileCommand>
{
    public UpdateCurrentUserProfileCommandValidator()
    {
        RuleFor(command => command.FullName).MaximumLength(100);
    }
}

public sealed class UpdateCurrentUserProfileCommandHandler(
    AppDbContext dbContext,
    ICurrentUser currentUser) : IRequestHandler<UpdateCurrentUserProfileCommand, Result<UserBasicInfo>>
{
    public async Task<Result<UserBasicInfo>> Handle(
        UpdateCurrentUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(
            candidate => candidate.EntityId == currentUser.UserEntityId && !candidate.IsDeleted,
            cancellationToken);

        if (user is null)
        {
            return Result<UserBasicInfo>.Failure(new Error("users.user_notFound", "User not found"));
        }

        user.FullName = string.IsNullOrWhiteSpace(request.FullName)
            ? null
            : request.FullName.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<UserBasicInfo>.Success(new UserBasicInfo(
            user.EntityId,
            user.Username,
            user.FullName,
            user.Email,
            user.AvatarUrl,
            user.LastSignInAt,
            user.IsEmailVerified));
    }
}

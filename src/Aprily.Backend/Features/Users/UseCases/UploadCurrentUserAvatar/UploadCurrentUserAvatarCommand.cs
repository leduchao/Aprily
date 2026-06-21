using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Features.Users.Models;
using Aprily.Backend.Features.Users.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Users.UseCases.UploadCurrentUserAvatar;

public sealed record UploadCurrentUserAvatarCommand(IFormFile Avatar) : IRequest<Result<UserBasicInfo>>;

public sealed class UploadCurrentUserAvatarCommandHandler(
    AppDbContext dbContext,
    ICurrentUser currentUser,
    IWebHostEnvironment environment) : IRequestHandler<UploadCurrentUserAvatarCommand, Result<UserBasicInfo>>
{
    private const long MaxAvatarSize = 5 * 1024 * 1024;

    private static readonly Dictionary<string, string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/webp"] = ".webp",
            ["image/gif"] = ".gif"
        };

    public async Task<Result<UserBasicInfo>> Handle(
        UploadCurrentUserAvatarCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Avatar.Length == 0 || request.Avatar.Length > MaxAvatarSize)
        {
            return Result<UserBasicInfo>.Failure(
                new Error("users.invalid_avatar", "Avatar must be a non-empty image smaller than 5 MB"));
        }

        if (!AllowedContentTypes.TryGetValue(request.Avatar.ContentType, out var extension))
        {
            return Result<UserBasicInfo>.Failure(
                new Error("users.invalid_avatar", "Avatar must be a JPG, PNG, WEBP, or GIF image"));
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(
            candidate => candidate.EntityId == currentUser.UserEntityId && !candidate.IsDeleted,
            cancellationToken);

        if (user is null)
        {
            return Result<UserBasicInfo>.Failure(new Error("users.user_notFound", "User not found"));
        }

        var avatarsDirectory = Path.Combine(environment.ContentRootPath, "Uploads", "Avatars");
        Directory.CreateDirectory(avatarsDirectory);

        var fileName = $"{user.EntityId:N}-{Guid.NewGuid():N}{extension}";
        var destinationPath = Path.Combine(avatarsDirectory, fileName);

        await using (var stream = new FileStream(
            destinationPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            FileOptions.Asynchronous))
        {
            await request.Avatar.CopyToAsync(stream, cancellationToken);
        }

        var oldAvatarUrl = user.AvatarUrl;
        user.AvatarUrl = $"/api/uploads/avatars/{fileName}";

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            File.Delete(destinationPath);
            throw;
        }

        DeleteOldLocalAvatar(oldAvatarUrl, avatarsDirectory);

        return Result<UserBasicInfo>.Success(new UserBasicInfo(
            user.EntityId,
            user.Username,
            user.FullName,
            user.Email,
            user.AvatarUrl,
            user.LastSignInAt,
            user.IsEmailVerified));
    }

    private static void DeleteOldLocalAvatar(string? avatarUrl, string avatarsDirectory)
    {
        const string localAvatarPrefix = "/api/uploads/avatars/";
        if (avatarUrl is null || !avatarUrl.StartsWith(localAvatarPrefix, StringComparison.Ordinal))
        {
            return;
        }

        var oldFileName = Path.GetFileName(avatarUrl);
        var oldFilePath = Path.Combine(avatarsDirectory, oldFileName);

        if (File.Exists(oldFilePath))
        {
            File.Delete(oldFilePath);
        }
    }
}

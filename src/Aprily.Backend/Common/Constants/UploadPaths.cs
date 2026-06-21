namespace Aprily.Backend.Common.Constants;

public static class UploadPaths
{
    public const string RootDirectoryName = "Uploads";
    public const string AvatarsDirectoryName = "Avatars";
    public const string ChatImagesDirectoryName = "ChatImages";

    public const string BaseRequestPath = "/api/uploads";
    public const string AvatarsRequestPath = $"{BaseRequestPath}/avatars";
    public const string ChatImagesRequestPath = $"{BaseRequestPath}/chat-images";

    public static IReadOnlyDictionary<string, string> PublicDirectories { get; } =
        new Dictionary<string, string>
        {
            [AvatarsRequestPath] = AvatarsDirectoryName,
            [ChatImagesRequestPath] = ChatImagesDirectoryName
        };

    public static string GetAvatarUrl(string fileName) => $"{AvatarsRequestPath}/{fileName}";

    public static string GetChatImageUrl(string fileName) => $"{ChatImagesRequestPath}/{fileName}";
}

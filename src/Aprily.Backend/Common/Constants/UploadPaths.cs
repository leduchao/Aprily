namespace Aprily.Backend.Common.Constants;

public static class UploadPaths
{
    public const string RootDirectoryName = "Uploads";
    public const string AvatarsDirectoryName = "Avatars";

    public const string BaseRequestPath = "/api/uploads";
    public const string AvatarsRequestPath = $"{BaseRequestPath}/avatars";

    public static string GetAvatarUrl(string fileName) => $"{AvatarsRequestPath}/{fileName}";
}

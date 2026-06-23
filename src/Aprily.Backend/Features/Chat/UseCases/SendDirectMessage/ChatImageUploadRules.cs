namespace Aprily.Backend.Features.Chat.UseCases.SendDirectMessage;

internal static class ChatImageUploadRules
{
    public const int MaxImageCount = 4;
    public const long MaxImageSize = 10 * 1024 * 1024;

    private static readonly IReadOnlyDictionary<string, string> AllowedContentTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/webp"] = ".webp",
            ["image/gif"] = ".gif"
        };

    public static bool IsAllowedContentType(string contentType) =>
        AllowedContentTypes.ContainsKey(contentType);

    public static string GetExtension(string contentType) => AllowedContentTypes[contentType];
}

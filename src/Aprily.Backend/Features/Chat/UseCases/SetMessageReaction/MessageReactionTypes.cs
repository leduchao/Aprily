namespace Aprily.Backend.Features.Chat.UseCases.SetMessageReaction;

public static class MessageReactionTypes
{
    public static IReadOnlySet<string> Allowed { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "like",
        "love",
        "haha",
        "sad",
        "wow",
        "angry"
    };
}

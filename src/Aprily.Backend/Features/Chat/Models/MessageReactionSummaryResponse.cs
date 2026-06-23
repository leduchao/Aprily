namespace Aprily.Backend.Features.Chat.Models;

public record MessageReactionSummaryResponse(
    string Type,
    int Count,
    bool ReactedByMe);

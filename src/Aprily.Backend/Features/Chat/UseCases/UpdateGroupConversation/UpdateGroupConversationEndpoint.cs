using Aprily.Backend.Common.Extensions;
using MediatR;
using System.Text.Json;

namespace Aprily.Backend.Features.Chat.UseCases.UpdateGroupConversation;

public static class UpdateGroupConversationEndpoint
{
    public static void MapUpdateGroupConversationEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/group-conversations/{conversationId:guid}", async (Guid conversationId, JsonElement request, ISender sender, CancellationToken cancellationToken) =>
        {
            var name = GetStringProperty(request, "name") ?? string.Empty;
            var hasAvatarUrl = TryGetProperty(request, "avatarUrl", out var avatarUrlElement);
            var avatarUrl = hasAvatarUrl && avatarUrlElement.ValueKind != JsonValueKind.Null
                ? avatarUrlElement.GetString()
                : null;

            var result = await sender.Send(new UpdateGroupConversationCommand(conversationId, name, avatarUrl, hasAvatarUrl), cancellationToken);
            return result.ToHttpResult();
        });
    }

    private static string? GetStringProperty(JsonElement element, string propertyName)
    {
        return TryGetProperty(element, propertyName, out var property) ? property.GetString() : null;
    }

    private static bool TryGetProperty(JsonElement element, string propertyName, out JsonElement property)
    {
        if (element.TryGetProperty(propertyName, out property))
            return true;

        return element.TryGetProperty(char.ToUpperInvariant(propertyName[0]) + propertyName[1..], out property);
    }
}

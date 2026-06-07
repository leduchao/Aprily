using Aprily.Application.Abstractions.Services;
using Aprily.Application.Chat;
using Aprily.Application.Chat.GetDirectMessages;
using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Api.Endpoints.Chat;

internal sealed class GetDirectMessages : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapChat()
            .MapGet(
                "/direct-messages/{otherUserId:guid}",
                async (
                    Guid otherUserId,
                    int? take,
                    DateTime? before,
                    IUserContext userContext,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    int pageSize = take ?? 50;
                    var query = new GetDirectMessagesQuery(
                        userContext.UserEntityId,
                        otherUserId,
                        pageSize,
                        before);
                    Result<IReadOnlyList<ChatMessageResponse>> result =
                        await sender.Send(query, cancellationToken);

                    if (result.IsFailure)
                    {
                        return result.Error?.Code == "chat.user_not_found"
                            ? Results.NotFound(result)
                            : Results.BadRequest(result);
                    }

                    return Results.Ok(result);
                })
            .WithName(nameof(GetDirectMessages));
    }
}

using Aprily.Application.Abstractions.Services;
using Aprily.Application.Chat;
using Aprily.Application.Chat.GetDirectConversations;
using Aprily.SharedKernel;

using MediatR;

namespace Aprily.Api.Endpoints.Chat;

internal sealed class GetDirectConversations : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapChat()
            .MapGet(
                "/conversations",
                async (
                    int? take,
                    DateTime? before,
                    IUserContext userContext,
                    ISender sender,
                    CancellationToken cancellationToken) =>
                {
                    int pageSize = take ?? 20;
                    var query = new GetDirectConversationsQuery(
                        userContext.UserEntityId,
                        pageSize,
                        before);
                    Result<IReadOnlyList<DirectConversationResponse>> result =
                        await sender.Send(query, cancellationToken);

                    if (result.IsFailure)
                    {
                        return result.Error?.Code == "chat.user_not_found"
                            ? Results.NotFound(result)
                            : Results.BadRequest(result);
                    }

                    return Results.Ok(result);
                })
            .WithName(nameof(GetDirectConversations));
    }
}

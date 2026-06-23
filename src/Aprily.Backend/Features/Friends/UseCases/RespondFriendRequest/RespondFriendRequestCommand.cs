using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Entities;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Friends.Models;
using Aprily.Backend.Features.Users.Services;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Friends.UseCases.RespondFriendRequest;

public sealed class RespondFriendRequestCommand(Guid requestId, FriendRequestDecision decision)
    : IRequest<Result<FriendRequestResponse>>
{
    public Guid RequestId { get; init; } = requestId;
    public FriendRequestDecision Decision { get; init; } = decision;

    public sealed class Handler(
        AppDbContext dbContext,
        ICurrentUser currentUser,
        IHubContext<ChatHub> chatHub)
        : IRequestHandler<RespondFriendRequestCommand, Result<FriendRequestResponse>>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly IHubContext<ChatHub> _chatHub = chatHub;

        public async Task<Result<FriendRequestResponse>> Handle(
            RespondFriendRequestCommand request,
            CancellationToken cancellationToken)
        {
            var friendRequest = await _dbContext.FriendRequests
                .Include(candidate => candidate.RequesterUser)
                .Include(candidate => candidate.AddresseeUser)
                .FirstOrDefaultAsync(
                    candidate => candidate.EntityId == request.RequestId &&
                        candidate.AddresseeUser.EntityId == _currentUser.UserEntityId &&
                        candidate.Status == "pending" &&
                        !candidate.IsDeleted &&
                        !candidate.RequesterUser.IsDeleted &&
                        !candidate.AddresseeUser.IsDeleted,
                    cancellationToken);

            if (friendRequest is null)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("friends.request_not_found", "Friend request not found"));
            }

            var now = DateTime.UtcNow;
            friendRequest.Status = request.Decision == FriendRequestDecision.Accept
                ? "accepted"
                : "declined";
            friendRequest.RespondedAt = now;

            if (request.Decision == FriendRequestDecision.Accept)
            {
                var (userLowId, userHighId) =
                    friendRequest.RequesterUserId < friendRequest.AddresseeUserId
                        ? (friendRequest.RequesterUserId, friendRequest.AddresseeUserId)
                        : (friendRequest.AddresseeUserId, friendRequest.RequesterUserId);

                var friendshipExists = await _dbContext.Friendships.AnyAsync(
                    friendship => friendship.UserLowId == userLowId &&
                        friendship.UserHighId == userHighId &&
                        !friendship.IsDeleted,
                    cancellationToken);

                if (!friendshipExists)
                {
                    await _dbContext.Friendships.AddAsync(
                        new Friendship
                        {
                            EntityId = Guid.NewGuid(),
                            UserLowId = userLowId,
                            UserHighId = userHighId,
                            AcceptedRequest = friendRequest,
                            IsDeleted = false
                        },
                        cancellationToken);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = friendRequest.ToResponse();

            await _chatHub.Clients
                .Groups(
                    ChatHub.UserGroup(response.Requester.Id),
                    ChatHub.UserGroup(response.Addressee.Id))
                .SendAsync(
                    ChatHub.FriendRequestsUpdatedEvent,
                    response,
                    cancellationToken);

            return Result<FriendRequestResponse>.Success(response);
        }
    }
}

public enum FriendRequestDecision
{
    Accept,
    Decline
}

using Aprily.Backend.Common.Results;
using Aprily.Backend.Database;
using Aprily.Backend.Entities;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Friends.Models;
using Aprily.Backend.Features.Users.Services;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Aprily.Backend.Features.Friends.UseCases.SendFriendRequest;

public sealed class SendFriendRequestCommand(Guid? recipientUserId, string? email)
    : IRequest<Result<FriendRequestResponse>>
{
    public Guid? RecipientUserId { get; init; } = recipientUserId;
    public string? Email { get; init; } = email;

    public sealed class Handler(
        AppDbContext dbContext,
        ICurrentUser currentUser,
        IHubContext<ChatHub> chatHub)
        : IRequestHandler<SendFriendRequestCommand, Result<FriendRequestResponse>>
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly IHubContext<ChatHub> _chatHub = chatHub;

        public async Task<Result<FriendRequestResponse>> Handle(
            SendFriendRequestCommand request,
            CancellationToken cancellationToken)
        {
            var requester = await _dbContext.Users
                .FirstOrDefaultAsync(
                    user => user.EntityId == _currentUser.UserEntityId && !user.IsDeleted,
                    cancellationToken);

            if (requester is null)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("users.user_notFound", "User not found"));
            }

            var addressee = request.RecipientUserId is not null
                ? await _dbContext.Users.FirstOrDefaultAsync(
                    user => user.EntityId == request.RecipientUserId.Value && !user.IsDeleted,
                    cancellationToken)
                : await _dbContext.Users.FirstOrDefaultAsync(
                    user => user.Email.ToLower() == request.Email!.Trim().ToLower() && !user.IsDeleted,
                    cancellationToken);

            if (addressee is null)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("friends.user_not_found", "User not found"));
            }

            if (requester.Id == addressee.Id)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("friends.cannot_add_self", "Cannot send a friend request to yourself"));
            }

            var (userLowId, userHighId) = requester.Id < addressee.Id
                ? (requester.Id, addressee.Id)
                : (addressee.Id, requester.Id);

            var alreadyFriends = await _dbContext.Friendships.AnyAsync(
                friendship => friendship.UserLowId == userLowId &&
                    friendship.UserHighId == userHighId &&
                    !friendship.IsDeleted,
                cancellationToken);

            if (alreadyFriends)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("friends.already_friends", "Users are already friends"));
            }

            var requestAlreadyPending = await _dbContext.FriendRequests.AnyAsync(
                friendRequest => friendRequest.UserLowId == userLowId &&
                    friendRequest.UserHighId == userHighId &&
                    friendRequest.Status == "pending" &&
                    !friendRequest.IsDeleted,
                cancellationToken);

            if (requestAlreadyPending)
            {
                return Result<FriendRequestResponse>.Failure(
                    new Error("friends.request_already_pending", "A friend request is already pending"));
            }

            var friendRequest = new FriendRequest
            {
                EntityId = Guid.NewGuid(),
                RequesterUserId = requester.Id,
                RequesterUser = requester,
                AddresseeUserId = addressee.Id,
                AddresseeUser = addressee,
                Status = "pending",
                IsDeleted = false
            };

            await _dbContext.FriendRequests.AddAsync(friendRequest, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = friendRequest.ToResponse();

            await _chatHub.Clients
                .Groups(
                    ChatHub.UserGroup(requester.EntityId),
                    ChatHub.UserGroup(addressee.EntityId))
                .SendAsync(
                    ChatHub.FriendRequestsUpdatedEvent,
                    response,
                    cancellationToken);

            return Result<FriendRequestResponse>.Success(response);
        }
    }
}

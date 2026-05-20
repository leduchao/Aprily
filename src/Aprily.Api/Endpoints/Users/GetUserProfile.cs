using Aprily.Application.Users.GetUserProfile;

using MediatR;

namespace Aprily.Api.Endpoints.Users;

public class GetProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet($"{BaseApiEndpoint.BasePath}/users/get-user-profile", async (
            string email,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetUserProfileQuery(email);
            var result = await sender.Send(query, ct);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        });
    }
}

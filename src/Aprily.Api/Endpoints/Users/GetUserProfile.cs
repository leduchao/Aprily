using Aprily.Application.Abstractions.Cqrs;
using Aprily.Application.Users.GetUserProfile;
using Aprily.SharedKernel;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Aprily.Api.Endpoints.Users;

public class GetProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet($"{BaseApiEndpoint.BasePath}/users/get-user-profile", async (
            [FromQuery] string email,
            [FromServices] ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetUserProfileQuery(email);
            var result = await sender.Send(query, ct);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        });
    }
}

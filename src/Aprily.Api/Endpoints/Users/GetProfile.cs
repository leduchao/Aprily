using Aprily.Application.Abstractions.Cqrs;
using Aprily.Application.Users;

using Microsoft.AspNetCore.Mvc;

namespace Aprily.Api.Endpoints.Users;

public class GetProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/users/get-profile", async (
            [FromQuery] string email,
            [FromServices] IQueryHandler<GetProfileQuery, UserProfileResponse> handler,
            CancellationToken ct) =>
        {
            var query = new GetProfileQuery(email);
            var result = await handler.Handle(query, ct);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        });
    }
}

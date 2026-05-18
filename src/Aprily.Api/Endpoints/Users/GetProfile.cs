using Aprily.Application.Abstractions.Cqrs;
using Aprily.Application.Users;
using Aprily.SharedKernel;

using Microsoft.AspNetCore.Mvc;

namespace Aprily.Api.Endpoints.Users;

public class GetProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet($"{BaseApiEndpoint.BasePath}/users/get-profile", async (
            [FromQuery] string email,
            [FromServices] IQueryHandler<GetProfileQuery, UserProfileResponse> handler,
            CancellationToken ct) =>
        {
            if (string.IsNullOrEmpty(email))
            {
                return Results.BadRequest(Result.Failure(new Error("user.email_required", "Email is required")));
            }

            var query = new GetProfileQuery(email);
            var result = await handler.Handle(query, ct);

            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        });
    }
}

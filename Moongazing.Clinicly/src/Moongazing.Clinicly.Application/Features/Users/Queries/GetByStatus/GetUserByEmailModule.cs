using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moongazing.Clinicly.Application.Features.Users.Queries.GetByEmail;

namespace Moongazing.Clinicly.Application.Features.Users.Queries.GetByStatus;

public class GetUserByStatusModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clinicly/users/by-status", async ([AsParameters] GetUserByStatusQuery request, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetUserByStatusQuery { Status = request.Status };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetUserByStatus")
        .WithTags("Users")
        .Produces<GetUserByStatusResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get user by status")
        .WithDescription("Returns a user's details using their status via query string.");
    }
}
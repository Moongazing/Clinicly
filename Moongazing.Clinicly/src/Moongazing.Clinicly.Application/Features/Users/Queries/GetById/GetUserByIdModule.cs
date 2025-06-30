using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.Users.Queries.GetById;

public class GetUserByIdModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clinicly/users/by-id", async ([AsParameters] GetUserByIdQuery request, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetUserByIdQuery { Id = request.Id };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetUserById")
        .WithTags("Users")
        .Produces<GetUserByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get user by id")
        .WithDescription("Returns a user's details using their id via query string.");
    }
}
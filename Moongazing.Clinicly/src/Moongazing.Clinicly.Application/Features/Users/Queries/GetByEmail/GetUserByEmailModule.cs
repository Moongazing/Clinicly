using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.Users.Queries.GetByEmail;


public class GetUserByEmailModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clinicly/users/by-email", async ([AsParameters] GetUserByEmailQuery request, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetUserByEmailQuery { Email = request.Email };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetUserByEmail")
        .WithTags("Users")
        .Produces<GetUserByEmailResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get user by email")
        .WithDescription("Returns a user's details using their email address via query string.");
    }
}
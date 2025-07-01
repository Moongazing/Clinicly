using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.Users.Commands.Create;

public class CreateUserModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/clinicly/users", async (CreateUserCommand command,
                                                  ISender sender,
                                                  CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Created(string.Empty, result);
        })
        .WithName("CreateUser")
        .WithTags("Users")
        .Produces<CreateUserResponse>(StatusCodes.Status201Created)
        .WithSummary("Create a new user")
        .WithDescription("Creates a user in the system with hashed password.");
    }
}

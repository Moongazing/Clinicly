using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.Users.Commands.Update;

public class UpdateUserModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/clinicly/users", async (UpdateUserCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("UpdateUser")
        .WithTags("Users")
        .Produces<UpdateUserResponse>(StatusCodes.Status200OK)
        .WithSummary("Update an existing user")
        .WithDescription("Updates user fields including hashed password.");
    }
}

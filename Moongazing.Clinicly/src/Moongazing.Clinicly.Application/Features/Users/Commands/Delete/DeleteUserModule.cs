using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.Users.Commands.Delete;

public class DeleteUserModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/clinicly/users/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("DeleteUser")
        .WithTags("User")
        .Produces<DeleteUserResponse>(StatusCodes.Status200OK)
        .WithSummary("Delete a user by ID")
        .WithDescription("Soft deletes a user by ID.");
    }
}

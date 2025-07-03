using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Commands.Delete;

public class DeleteUserOperationClaimModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/clinicly/user-operation-claims/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteUserOperationClaimCommand { Id = id };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("DeleteUserOperationClaim")
        .WithTags("UserOperationClaims")
        .Produces<DeleteUserOperationClaimCommandResponse>(StatusCodes.Status200OK)
        .WithSummary("Delete a user-operation claim by ID")
        .WithDescription("Soft deletes the relation between a user and an operation claim.");
    }
}

using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moongazing.Clinicly.Application.Features.Users.Commands.Delete;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Commands.Delete;

public class DeleteOperationClaimModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/clinicly/operation-claims/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("DeleteOperationClaim")
        .WithTags("OperationClaims")
        .Produces<DeleteOperationClaimResponse>(StatusCodes.Status200OK)
        .WithSummary("Delete a operation claim by ID")
        .WithDescription("Soft deletes a operation claim by ID.");
    }
}
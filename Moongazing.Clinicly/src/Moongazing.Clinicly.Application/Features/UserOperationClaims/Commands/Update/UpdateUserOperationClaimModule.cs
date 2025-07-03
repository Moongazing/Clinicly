using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Commands.Update;

public class UpdateUserOperationClaimModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/clinicly/user-operation-claims", async (
            UpdateUserOperationClaimCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("UpdateUserOperationClaim")
        .WithTags("UserOperationClaims")
        .Produces<UpdateUserOperationClaimCommandResponse>(StatusCodes.Status200OK)
        .WithSummary("Update an existing user-operation claim")
        .WithDescription("Updates the user-operation claim relation by changing the assigned role.");
    }
}

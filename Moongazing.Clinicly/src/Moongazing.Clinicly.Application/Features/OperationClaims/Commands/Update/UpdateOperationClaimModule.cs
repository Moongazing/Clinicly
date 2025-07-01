using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Commands.Update;

public class UpdateOperationClaimModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/clinicly/operation-claims", async (UpdateOperationClaimCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("UpdateOperationClaim")
        .WithTags("OperationClaims")
        .Produces<UpdateOperationClaimResponse>(StatusCodes.Status200OK)
        .WithSummary("Update an existing operation claim")
        .WithDescription("Updates opearetion claim fields.");
    }
}
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Commands.Create;

public class CreateOperationClaimModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/clinicly/operation-claims", async (
            CreateOperationClaimCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Created(string.Empty, result);
        })
        .WithName("CreateOperationClaim")
        .WithTags("OperationClaims")
        .Produces<CreateOperationClaimResponse>(StatusCodes.Status201Created)
        .WithSummary("Create a new operation claim")
        .WithDescription("Creates a new role/operation claim in the system.");
    }
}

using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Commands.Create;

public class CreateUserOperationClaimModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/clinicly/user-operation-claims", async (
            CreateUserOperationClaimCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return Results.Created($"/api/clinicly/user-operation-claims/{result.Id}", result);
        })
        .WithName("CreateUserOperationClaim")
        .WithTags("UserOperationClaims")
        .Produces<CreateUserOperationClaimCommandResponse>(StatusCodes.Status201Created)
        .WithSummary("Assign an operation claim to a user")
        .WithDescription("Creates a user-operation claim record by assigning an existing role to a user.");
    }
}
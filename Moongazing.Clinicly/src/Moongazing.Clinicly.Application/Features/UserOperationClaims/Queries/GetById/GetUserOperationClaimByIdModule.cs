using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetById;

public class GetUserOperationClaimByIdModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clinicly/user-operation-claims/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserOperationClaimByIdQuery { Id = id };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetUserOperationClaimById")
        .WithTags("UserOperationClaims")
        .Produces<GetUserOperationClaimByIdResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithSummary("Get a user-operation claim by ID")
        .WithDescription("Returns the operation claim assigned to the specified user, including full user and claim details.");
    }
}

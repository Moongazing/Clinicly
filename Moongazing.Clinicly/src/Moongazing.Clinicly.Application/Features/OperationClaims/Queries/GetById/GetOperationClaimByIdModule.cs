using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetById;

public class GetOperationClaimByIdModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clinicly/operation-claim/by-id", async ([AsParameters] GetOperationClaimByIdQuery request, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetOperationClaimByIdQuery { Id = request.Id };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetOperationClaimById")
        .WithTags("OperationClaims")
        .Produces<GetOperationClaimByIdResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get operation claim by id")
        .WithDescription("Returns a operation claim's details using their id via query string.");
    }
}
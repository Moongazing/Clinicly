using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetByName;

public class GetOperationClaimByNameModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clinicly/operation-claim/by-name", async ([AsParameters] GetOperationClaimByNameQuery request, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new GetOperationClaimByNameQuery { Name = request.Name };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetOperationClaimByName")
        .WithTags("OperationClaims")
        .Produces<GetOperationClaimByNameResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Get operation claim by name")
        .WithDescription("Returns a operation claim's details using their name via query string.");
    }
}
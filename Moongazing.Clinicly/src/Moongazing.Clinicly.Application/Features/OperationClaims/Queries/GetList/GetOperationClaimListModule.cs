using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moongazing.Kernel.Application.Responses;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetList;

public class GetOperationClaimListModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clinicly/operation-claims", async (
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetOperationClaimListQuery
            {
                PageRequest =
                {
                PageIndex = pageIndex,
                PageSize= pageSize
                }
            };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetOperationClaimList")
        .WithTags("OperationClaims")
        .Produces<PaginatedResponse<GetOperationClaimListResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get paginated list of users")
        .WithDescription("Returns a paginated list of operation claim based on given PageRequest parameters (PageIndex(starts from 0), PageSize).");
    }
}
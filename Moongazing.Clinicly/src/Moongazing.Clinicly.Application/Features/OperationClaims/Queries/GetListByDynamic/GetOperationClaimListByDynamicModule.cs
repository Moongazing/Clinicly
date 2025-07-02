using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moongazing.Kernel.Application.Requests;
using Moongazing.Kernel.Application.Responses;
using Moongazing.Kernel.Persistence.Dynamic;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetListByDynamic;

public class GetOperationClaimListByDynamicModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/clinicly/operation-claims/filter", async (
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            [FromBody] DynamicQuery dynamicQuery,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetOperationClaimListByDynamicQuery
            {
                PageRequest = new PageRequest
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                },
                DynamicQuery = dynamicQuery
            };

            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetOperationClaimListByDynamic")
        .WithTags("OperationClaims")
        .Produces<PaginatedResponse<GetOperationClaimListByDynamicResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Search users with dynamic filters")
        .WithDescription("Returns a paginated list of users based on dynamic filtering criteria. Pagination parameters are passed via query.");
    }
}
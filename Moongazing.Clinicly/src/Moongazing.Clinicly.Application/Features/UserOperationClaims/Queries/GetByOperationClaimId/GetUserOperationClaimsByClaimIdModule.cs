using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetByClaimId;
using Moongazing.Kernel.Application.Requests;
using Moongazing.Kernel.Application.Responses;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetByOperationClaimId;

public class GetUserOperationClaimsByClaimIdModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clinicly/user-operation-claims/by-claim/{operationClaimId:guid}", async (
            Guid operationClaimId,
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserOperationClaimsByClaimIdQuery
            {
                OperationClaimId = operationClaimId,
                PageRequest = new PageRequest
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            };

            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetUserOperationClaimsByClaimId")
        .WithTags("UserOperationClaims")
        .Produces<PaginatedResponse<GetUserOperationClaimsByClaimIdResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get paginated list of user-operation claims by OperationClaimId")
        .WithDescription("Returns a paginated list of users assigned to a specific operation claim. PageIndex starts from 0.");
    }
}
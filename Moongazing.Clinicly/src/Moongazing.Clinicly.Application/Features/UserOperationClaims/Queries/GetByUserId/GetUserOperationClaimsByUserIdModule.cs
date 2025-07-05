using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetByOperationClaimId;
using Moongazing.Kernel.Application.Requests;
using Moongazing.Kernel.Application.Responses;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetByUserId;

public class GetUserOperationClaimsByUserIdModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clinicly/user-operation-claims/by-user/{userId:guid}", async (
            Guid userId,
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserOperationClaimsByUserIdQuery
            {
                UserId = userId,
                PageRequest = new PageRequest
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            };

            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetUserOperationClaimsByUserId")
        .WithTags("UserOperationClaims")
        .Produces<PaginatedResponse<GetUserOperationClaimsByUserIdResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get paginated list of user-operation claims by user id")
        .WithDescription("Returns a paginated list of users assigned to a specific operation claim. PageIndex starts from 0.");
    }
}
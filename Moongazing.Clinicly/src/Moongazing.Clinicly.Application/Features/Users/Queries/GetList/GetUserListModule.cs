using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moongazing.Kernel.Application.Responses;

namespace Moongazing.Clinicly.Application.Features.Users.Queries.GetList;

public class GetUserListModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/clinicly/users", async (
            [FromQuery] int pageIndex,
            [FromQuery] int pageSize,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserListQuery
            { PageRequest =
                {
                    PageIndex = pageIndex,
                    PageSize= pageSize
                } 
            };
            var result = await sender.Send(query, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetUserList")
        .WithTags("Users")
        .Produces<PaginatedResponse<GetUserListResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get paginated list of users")
        .WithDescription("Returns a paginated list of users based on given PageRequest parameters (PageIndex(starts from 0), PageSize).");
    }
}

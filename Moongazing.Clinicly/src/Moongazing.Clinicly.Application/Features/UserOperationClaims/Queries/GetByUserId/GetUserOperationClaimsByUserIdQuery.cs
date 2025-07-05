using MediatR;
using Microsoft.EntityFrameworkCore;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Constants;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Requests;
using Moongazing.Kernel.Application.Responses;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetByUserId;

public class GetUserOperationClaimsByUserIdQuery : IRequest<PaginatedResponse<GetUserOperationClaimsByUserIdResponse>>,
    ILoggableRequest,
    IIntervalRequest,
    IRateLimitedRequest,
    ICachableRequest,
    ISecuredRequest
{
    public Guid UserId { get; set; }
    public PageRequest PageRequest { get; set; } = default!;
    public bool BypassCache { get; }
    public string? CacheGroupKey => UserOperationClaimsMessages.SectionName;
    public string CacheKey => $"{GetType().Name}({UserId}-{PageRequest.PageIndex}-{PageRequest.PageSize})";
    public TimeSpan? SlidingExpiration { get; }
    public string[] Roles => [GeneralOperationClaims.Read];
    public int Interval => 15;

    public class GetUserOperationClaimsByUserIdQueryHandler
        : IRequestHandler<GetUserOperationClaimsByUserIdQuery, PaginatedResponse<GetUserOperationClaimsByUserIdResponse>>
    {
        private readonly IUserOperationClaimRepository userOperationClaimRepository;

        public GetUserOperationClaimsByUserIdQueryHandler(IUserOperationClaimRepository userOperationClaimRepository)
        {
            this.userOperationClaimRepository = userOperationClaimRepository;
        }

        public async Task<PaginatedResponse<GetUserOperationClaimsByUserIdResponse>> Handle(
            GetUserOperationClaimsByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            var pagedList = await userOperationClaimRepository
                .GetListAsync(
                    predicate: u => u.UserId == request.UserId,
                    index: request.PageRequest.PageIndex,
                    size: request.PageRequest.PageSize,
                    include: q => q.Include(x => x.User)
                                   .Include(x => x.OperationClaim),
                    cancellationToken: cancellationToken);


            var items = pagedList.Items.Select(entity => new GetUserOperationClaimsByUserIdResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                OperationClaimId = entity.OperationClaimId,
                OperationClaimName = entity.OperationClaim.Name,
                UserFullName = $"{entity.User.FirstName} {entity.User.LastName}"
            }).ToList();

            return new PaginatedResponse<GetUserOperationClaimsByUserIdResponse>
            {
                Items = items,
                Pages = pagedList.Pages,
                Count = pagedList.Count,
                Index = pagedList.Index,
                HasNext = pagedList.HasNext,
                HasPrevious = pagedList.HasPrevious
            };
        }
    }
}

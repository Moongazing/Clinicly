using MediatR;
using Microsoft.EntityFrameworkCore;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Constants;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetByClaimId;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Requests;
using Moongazing.Kernel.Application.Responses;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetByOperationClaimId;

public class GetUserOperationClaimsByClaimIdQuery : IRequest<PaginatedResponse<GetUserOperationClaimsByClaimIdResponse>>,
    ILoggableRequest,
    IIntervalRequest,
    IRateLimitedRequest,
    ICachableRequest
{
    public Guid OperationClaimId { get; set; }
    public PageRequest PageRequest { get; set; } = default!;
    public bool BypassCache { get; }
    public string? CacheGroupKey => UserOperationClaimsMessages.SectionName;
    public string CacheKey => $"{GetType().Name}({OperationClaimId}-{PageRequest.PageIndex}-{PageRequest.PageSize})";
    public TimeSpan? SlidingExpiration { get; }
    public string[] Roles => [GeneralOperationClaims.Read];
    public int Interval => 15;

    public class GetUserOperationClaimsByClaimIdQueryHandler
        : IRequestHandler<GetUserOperationClaimsByClaimIdQuery, PaginatedResponse<GetUserOperationClaimsByClaimIdResponse>>
    {
        private readonly IUserOperationClaimRepository userOperationClaimRepository;

        public GetUserOperationClaimsByClaimIdQueryHandler(IUserOperationClaimRepository userOperationClaimRepository)
        {
            this.userOperationClaimRepository = userOperationClaimRepository;
        }

        public async Task<PaginatedResponse<GetUserOperationClaimsByClaimIdResponse>> Handle(
            GetUserOperationClaimsByClaimIdQuery request,
            CancellationToken cancellationToken)
        {
            var pagedList = await userOperationClaimRepository
                .GetListAsync(
                    predicate: u => u.OperationClaimId == request.OperationClaimId,
                    index: request.PageRequest.PageIndex,
                    size: request.PageRequest.PageSize,
                    include: q => q.Include(x => x.User)
                                    .Include(x => x.OperationClaim),
                    cancellationToken: cancellationToken);


            var items = pagedList.Items.Select(entity => new GetUserOperationClaimsByClaimIdResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                OperationClaimId = entity.OperationClaimId,
                OperationClaimName = entity.OperationClaim.Name,
                UserFullName = $"{entity.User.FirstName} {entity.User.LastName}"
            }).ToList();

            return new PaginatedResponse<GetUserOperationClaimsByClaimIdResponse>
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

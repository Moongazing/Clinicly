using MediatR;
using Microsoft.EntityFrameworkCore;
using Moongazing.Clinicly.Application.Features.OperationClaims.Constants;
using Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetList;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Constants;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Requests;
using Moongazing.Kernel.Application.Responses;
using Moongazing.Kernel.Persistence.Paging;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetList;

public class GetUserOperationClaimListQuery : IRequest<PaginatedResponse<GetUserOperationClaimListResponse>>,
    ILoggableRequest,
    IIntervalRequest,
    ICachableRequest,
    IRateLimitedRequest,
    ISecuredRequest
{
    public PageRequest PageRequest { get; set; } = default!;
    public string[] Roles => [GeneralOperationClaims.Read];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UserOperationClaimsMessages.SectionName;
    public string CacheKey => $"{GetType().Name}({PageRequest.PageIndex}-{PageRequest.PageSize})";
    public TimeSpan? SlidingExpiration { get; }
    public int Interval => 15;


    public class GetUserOperationClaimListQueryHandler : IRequestHandler<GetUserOperationClaimListQuery, PaginatedResponse<GetUserOperationClaimListResponse>>
    {
        private readonly IUserOperationClaimRepository userOperationClaimRepository;

        public GetUserOperationClaimListQueryHandler(IUserOperationClaimRepository userOperationClaimRepository)
        {
            this.userOperationClaimRepository = userOperationClaimRepository;
        }

        public async Task<PaginatedResponse<GetUserOperationClaimListResponse>> Handle(GetUserOperationClaimListQuery request, CancellationToken cancellationToken)
        {
            IPaginate<UserOperationClaimEntity> userOperationClaims = await userOperationClaimRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                withDeleted: false,
                include: i => i.Include(u => u.OperationClaim)
                               .Include(u => u.User),
                cancellationToken: cancellationToken);

            var items = userOperationClaims.Items.Select(entity => new GetUserOperationClaimListResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                OperationClaimId = entity.OperationClaimId,
                OperationClaimName = entity.OperationClaim.Name,
                UserFullName = $"{entity.User.FirstName} {entity.User.LastName}"
            }).ToList();

            return new PaginatedResponse<GetUserOperationClaimListResponse>
            {
                Items = items,
                Pages = userOperationClaims.Pages,
                Count = userOperationClaims.Count,
                Index = userOperationClaims.Index,
                HasNext = userOperationClaims.HasNext,
                HasPrevious = userOperationClaims.HasPrevious
            };
        }
    }
}

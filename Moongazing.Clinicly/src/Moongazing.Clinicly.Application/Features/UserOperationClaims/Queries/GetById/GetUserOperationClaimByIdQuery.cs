using MediatR;
using Microsoft.EntityFrameworkCore;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Constants;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Rules;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetById;

public class GetUserOperationClaimByIdQuery : IRequest<GetUserOperationClaimByIdResponse>,
     ILoggableRequest,
     IIntervalRequest,
     ICachableRequest,
     ISecuredRequest,
     IRateLimitedRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [GeneralOperationClaims.Read];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UserOperationClaimsMessages.SectionName;
    public string CacheKey => $"{GetType().Name}({Id})";
    public TimeSpan? SlidingExpiration { get; }
    public int Interval => 15;

    public class GetUserOperationClaimByIdQueryHandler : IRequestHandler<GetUserOperationClaimByIdQuery, GetUserOperationClaimByIdResponse>
    {
        private readonly IUserOperationClaimRepository userOperationClaimRepository;
        private readonly UserOperationClaimBusinessRules userOperationClaimBusinessRules;

        public GetUserOperationClaimByIdQueryHandler(
            IUserOperationClaimRepository userOperationClaimRepository,
            UserOperationClaimBusinessRules userOperationClaimBusinessRules)
        {
            this.userOperationClaimRepository = userOperationClaimRepository;
            this.userOperationClaimBusinessRules = userOperationClaimBusinessRules;
        }

        public async Task<GetUserOperationClaimByIdResponse> Handle(GetUserOperationClaimByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await userOperationClaimRepository.GetAsync(
                predicate: u => u.Id == request.Id,
                withDeleted: false,
                include: x => x.Include(u => u.User)
                               .Include(u => u.OperationClaim),
                cancellationToken: cancellationToken);

            await userOperationClaimBusinessRules.UserOperationClaimShouldExistWhenSelected(entity);

            return new GetUserOperationClaimByIdResponse
            {
                Id = entity!.Id,
                UserId = entity.UserId,
                OperationClaimId = entity.OperationClaimId,
                OperationClaimName = entity.OperationClaim.Name,
                UserFullName = $"{entity.User.FirstName} {entity.User.LastName}"
            };
        }
    }
}

using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Features.OperationClaims.Constants;
using Moongazing.Clinicly.Application.Features.OperationClaims.Rules;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetByName;

public class GetOperationClaimByNameQuery : IRequest<GetOperationClaimByNameResponse>,
     ILoggableRequest,
    IIntervalRequest,
    ICachableRequest,
    ISecuredRequest,
    IRateLimitedRequest
{
    public string Name { get; set; } = default!;
    public string[] Roles => [GeneralOperationClaims.Read];
    public bool BypassCache { get; }
    public string? CacheGroupKey => OperationClaimMessages.SectionName;
    public string CacheKey => $"{GetType().Name}({Name.ToLower()})";
    public TimeSpan? SlidingExpiration { get; }
    public int Interval => 15;


    public class GetOperationClaimByNameQueryHandler : IRequestHandler<GetOperationClaimByNameQuery, GetOperationClaimByNameResponse>
    {
        private readonly IOperationClaimRepository operationClaimRepository;
        private readonly OperationClaimBusinessRules operationClaimBusinessRules;

        public GetOperationClaimByNameQueryHandler(IOperationClaimRepository operationClaimRepository,
                                                   OperationClaimBusinessRules operationClaimBusinessRules)
        {
            this.operationClaimRepository = operationClaimRepository;
            this.operationClaimBusinessRules = operationClaimBusinessRules;
        }

        public async Task<GetOperationClaimByNameResponse> Handle(GetOperationClaimByNameQuery request, CancellationToken cancellationToken)
        {
            OperationClaimEntity? operationClaim = await operationClaimRepository.GetAsync(predicate: u => u.Name == request.Name,
                                                                                           withDeleted: false,
                                                                                           cancellationToken: cancellationToken);

            await operationClaimBusinessRules.OperationClaimShouldBeExistsWhenSelected(operationClaim);

            GetOperationClaimByNameResponse response = operationClaim.Adapt<GetOperationClaimByNameResponse>();

            return response;
        }
    }

}

using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Features.OperationClaims.Rules;
using Moongazing.Clinicly.Application.Features.Users.Constants;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetById;

public class GetOperationClaimByIdQuery : IRequest<GetOperationClaimByIdResponse>,
     ILoggableRequest,
    IIntervalRequest,
    ICachableRequest,
    ISecuredRequest,
    IRateLimitedRequest
{
    public Guid Id { get; set; } = default!;
    public string[] Roles => [GeneralOperationClaims.Read];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UsersMessages.SectionName;
    public string CacheKey => $"{GetType().Name}({Id})";
    public TimeSpan? SlidingExpiration { get; }
    public int Interval => 15;


    public class GetOperationClaimByIdQueryHandler : IRequestHandler<GetOperationClaimByIdQuery, GetOperationClaimByIdResponse>
    {
        private readonly IOperationClaimRepository operationClaimRepository;
        private readonly OperationClaimBusinessRules operationClaimBusinessRules;

        public GetOperationClaimByIdQueryHandler(IOperationClaimRepository operationClaimRepository,
                                                 OperationClaimBusinessRules operationClaimBusinessRules)
        {
            this.operationClaimRepository = operationClaimRepository;
            this.operationClaimBusinessRules = operationClaimBusinessRules;
        }

        public async Task<GetOperationClaimByIdResponse> Handle(GetOperationClaimByIdQuery request, CancellationToken cancellationToken)
        {
            OperationClaimEntity? operationClaim = await operationClaimRepository.GetAsync(predicate: u => u.Id == request.Id,
                                                                                           withDeleted: false,
                                                                                           cancellationToken: cancellationToken);

            await operationClaimBusinessRules.OperationClaimShouldBeExistsWhenSelected(operationClaim);

            GetOperationClaimByIdResponse response = operationClaim.Adapt<GetOperationClaimByIdResponse>();

            return response;
        }
    }

}

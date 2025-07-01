using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Features.OperationClaims.Rules;
using Moongazing.Clinicly.Application.Features.Users.Commands.Create;
using Moongazing.Clinicly.Application.Features.Users.Constants;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Pipelines.Transaction;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Commands.Create;

public class CreateOperationClaimCommand : IRequest<CreateOperationClaimResponse>,
    ILoggableRequest,
    IIntervalRequest,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRateLimitedRequest,
    ISecuredRequest
{

    public string Name { get; set; } = default!;
    public string[] Roles => [GeneralOperationClaims.Add, GeneralOperationClaims.Write];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UsersMessages.SectionName;
    public string? CacheKey => null;
    public int Interval => 15;


    public class CreateOperationClaimCommandHandler : IRequestHandler<CreateOperationClaimCommand, CreateOperationClaimResponse>
    {
        private readonly IOperationClaimRepository operationClaimRepository;
        private readonly OperationClaimBusinessRules operationClaimBusinessRules;
        public CreateOperationClaimCommandHandler(IOperationClaimRepository operationClaimRepository,
                                                  OperationClaimBusinessRules operationClaimBusinessRules)
        {
            this.operationClaimRepository = operationClaimRepository;
            this.operationClaimBusinessRules = operationClaimBusinessRules;
        }
        public async Task<CreateOperationClaimResponse> Handle(CreateOperationClaimCommand request, CancellationToken cancellationToken)
        {
            await operationClaimBusinessRules.OperationClaimNameShouldNotExists(request.Name);

            OperationClaimEntity operationClaim = request.Adapt<OperationClaimEntity>();

            operationClaim.Id = Guid.NewGuid();

            OperationClaimEntity? createdOperationClaim = await operationClaimRepository.AddAsync(operationClaim, cancellationToken);

            CreateOperationClaimResponse response = createdOperationClaim.Adapt<CreateOperationClaimResponse>();

            return response;



        }
    }
}

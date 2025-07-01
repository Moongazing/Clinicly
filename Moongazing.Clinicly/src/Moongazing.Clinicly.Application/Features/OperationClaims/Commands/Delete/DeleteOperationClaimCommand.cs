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
using Moongazing.Kernel.Application.Pipelines.Transaction;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Commands.Delete;

public class DeleteOperationClaimCommand : IRequest<DeleteOperationClaimResponse>,
    ILoggableRequest,
    IIntervalRequest,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRateLimitedRequest,
    ISecuredRequest
{
    public Guid Id { get; set; }
    public string[] Roles => [GeneralOperationClaims.Delete, GeneralOperationClaims.Write];
    public bool BypassCache { get; }
    public string? CacheGroupKey => OperationClaimMessages.SectionName;
    public string? CacheKey => null;
    public int Interval => 15;

    public class DeleteOperationClaimCommandHandler : IRequestHandler<DeleteOperationClaimCommand, DeleteOperationClaimResponse>
    {
        private readonly IOperationClaimRepository operationClaimRepository;
        private readonly OperationClaimBusinessRules operationClaimBusinessRules;
        public DeleteOperationClaimCommandHandler(IOperationClaimRepository operationClaimRepository,
                                                  OperationClaimBusinessRules operationClaimBusinessRules)
        {
            this.operationClaimRepository = operationClaimRepository;
            this.operationClaimBusinessRules = operationClaimBusinessRules;
        }
        public async Task<DeleteOperationClaimResponse> Handle(DeleteOperationClaimCommand request, CancellationToken cancellationToken)
        {

            OperationClaimEntity? operationClaim = await operationClaimRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                                                           withDeleted: false,
                                                                                           cancellationToken: cancellationToken);
            await operationClaimBusinessRules.OperationClaimShouldBeExistsWhenSelected(operationClaim);

            await operationClaimRepository.DeleteAsync(operationClaim!, true, cancellationToken);

            DeleteOperationClaimResponse response = operationClaim.Adapt<DeleteOperationClaimResponse>();

            return response;
        }
    }
}


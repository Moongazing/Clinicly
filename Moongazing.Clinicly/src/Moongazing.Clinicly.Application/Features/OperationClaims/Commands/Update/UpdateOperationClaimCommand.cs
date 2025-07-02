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

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Commands.Update;

public class UpdateOperationClaimCommand : IRequest<UpdateOperationClaimResponse>,
    ILoggableRequest,
    IIntervalRequest,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRateLimitedRequest,
    ISecuredRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string[] Roles => [GeneralOperationClaims.Update, GeneralOperationClaims.Write];
    public bool BypassCache { get; }
    public string? CacheGroupKey => OperationClaimMessages.SectionName;
    public string? CacheKey => null;
    public int Interval => 15;


    public class UpdateOperationClaimCommandHandler : IRequestHandler<UpdateOperationClaimCommand, UpdateOperationClaimResponse>
    {
        private readonly IOperationClaimRepository operationClaimRepository;
        private readonly OperationClaimBusinessRules operationClaimBusinessRules;
        public UpdateOperationClaimCommandHandler(IOperationClaimRepository operationClaimRepository,
                                                  OperationClaimBusinessRules operationClaimBusinessRules)
        {
            this.operationClaimRepository = operationClaimRepository;
            this.operationClaimBusinessRules = operationClaimBusinessRules;
        }
        public async Task<UpdateOperationClaimResponse> Handle(UpdateOperationClaimCommand request, CancellationToken cancellationToken)
        {
            OperationClaimEntity? existingOperationClaim = await operationClaimRepository.GetAsync(predicate: x => x.Id == request.Id,
                                                                                                  withDeleted: false,
                                                                                                  cancellationToken: cancellationToken);

            await operationClaimBusinessRules.OperationClaimShouldBeExistsWhenSelected(existingOperationClaim);

            if (existingOperationClaim!.Name != request.Name)
            {
                await operationClaimBusinessRules.OperationClaimNameShouldNotExists(request.Name);
            }
            request.Adapt(existingOperationClaim);

            await operationClaimRepository.UpdateAsync(existingOperationClaim, cancellationToken);

            UpdateOperationClaimResponse response = existingOperationClaim.Adapt<UpdateOperationClaimResponse>();

            return response;
        }
    }

}

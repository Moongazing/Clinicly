using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Features.OperationClaims.Rules;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Constants;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Rules;
using Moongazing.Clinicly.Application.Features.Users.Rules;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Pipelines.Transaction;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Commands.Update;

public class UpdateUserOperationClaimCommand : IRequest<UpdateUserOperationClaimCommandResponse>,
    ILoggableRequest,
    IIntervalRequest,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRateLimitedRequest,
    ISecuredRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OperationClaimId { get; set; }

    public string[] Roles => [GeneralOperationClaims.Update, GeneralOperationClaims.Write];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UserOperationClaimsMessages.SectionName;
    public string? CacheKey => null;
    public int Interval => 15;

    public class UpdateUserOperationClaimCommandHandler
        : IRequestHandler<UpdateUserOperationClaimCommand, UpdateUserOperationClaimCommandResponse>
    {
        private readonly IUserOperationClaimRepository userOperationClaimRepository;
        private readonly UserOperationClaimBusinessRules userOperationClaimBusinessRules;
        private readonly UserBusinessRules userBusinessRules;
        private readonly OperationClaimBusinessRules operationClaimBusinessRules;

        public UpdateUserOperationClaimCommandHandler(
            IUserOperationClaimRepository userOperationClaimRepository,
            UserOperationClaimBusinessRules userOperationClaimBusinessRules,
            UserBusinessRules userBusinessRules,
            OperationClaimBusinessRules operationClaimBusinessRules)
        {
            this.userOperationClaimRepository = userOperationClaimRepository;
            this.userOperationClaimBusinessRules = userOperationClaimBusinessRules;
            this.userBusinessRules = userBusinessRules;
            this.operationClaimBusinessRules = operationClaimBusinessRules;
        }

        public async Task<UpdateUserOperationClaimCommandResponse> Handle(
            UpdateUserOperationClaimCommand request,
            CancellationToken cancellationToken)
        {
            var entity = await userOperationClaimRepository.GetAsync(
                predicate: x => x.Id == request.Id,
                withDeleted: false,
                cancellationToken: cancellationToken);

            await userOperationClaimBusinessRules.UserOperationClaimShouldExistWhenSelected(entity);
            await userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);
            await operationClaimBusinessRules.OperationClaimIdShouldBeExistsWhenSelected(request.OperationClaimId);

            if (entity!.UserId != request.UserId || entity.OperationClaimId != request.OperationClaimId)
            {
                await userOperationClaimBusinessRules
                    .UserShouldNotHaveOperationClaimAlreadyWhenInsert(request.UserId, request.OperationClaimId);
            }

            request.Adapt(entity);
            await userOperationClaimRepository.UpdateAsync(entity, cancellationToken);

            return entity.Adapt<UpdateUserOperationClaimCommandResponse>();
        }


    }
}

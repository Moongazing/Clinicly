using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Constants;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Rules;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Pipelines.Transaction;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Commands.Delete;

public class DeleteUserOperationClaimCommand : IRequest<DeleteUserOperationClaimCommandResponse>,
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
    public string? CacheGroupKey => UserOperationClaimsMessages.SectionName;
    public string? CacheKey => null;
    public int Interval => 15;

    public class DeleteUserOperationClaimCommandHandler
        : IRequestHandler<DeleteUserOperationClaimCommand, DeleteUserOperationClaimCommandResponse>
    {
        private readonly IUserOperationClaimRepository userOperationClaimRepository;
        private readonly UserOperationClaimBusinessRules userOperationClaimBusinessRules;

        public DeleteUserOperationClaimCommandHandler(
            IUserOperationClaimRepository userOperationClaimRepository,
            UserOperationClaimBusinessRules userOperationClaimBusinessRules)
        {
            this.userOperationClaimRepository = userOperationClaimRepository;
            this.userOperationClaimBusinessRules = userOperationClaimBusinessRules;
        }

        public async Task<DeleteUserOperationClaimCommandResponse> Handle(DeleteUserOperationClaimCommand request,
                                                                          CancellationToken cancellationToken)
        {
            UserOperationClaimEntity? entity = await userOperationClaimRepository.GetAsync(
                predicate: x => x.Id == request.Id,
                withDeleted: false,
                cancellationToken: cancellationToken);

            await userOperationClaimBusinessRules.UserOperationClaimShouldExistWhenSelected(entity);

            await userOperationClaimRepository.DeleteAsync(entity!, permanent: true, cancellationToken);

            return entity.Adapt<DeleteUserOperationClaimCommandResponse>();
        }
    }
}

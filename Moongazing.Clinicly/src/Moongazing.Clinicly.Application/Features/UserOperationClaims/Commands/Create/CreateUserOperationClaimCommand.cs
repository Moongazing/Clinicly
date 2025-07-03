using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moongazing.Clinicly.Application.Features.OperationClaims.Rules;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Constants;
using Moongazing.Clinicly.Application.Features.UserOperationClaims.Rules;
using Moongazing.Clinicly.Application.Features.Users.Rules;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Pipelines.Transaction;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Commands.Create;

public class CreateUserOperationClaimCommand:IRequest<CreateUserOperationClaimCommandResponse>,
    ILoggableRequest,
    IIntervalRequest,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRateLimitedRequest
{
    public Guid UserId { get; set; }
    public Guid OperationClaimId { get; set; }

    public string[] Roles => [GeneralOperationClaims.Add, GeneralOperationClaims.Write];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UserOperationClaimsMessages.SectionName;
    public string? CacheKey => null;
    public int Interval => 15;

    public class CreateUserOperationClaimCommandHandler
      : IRequestHandler<CreateUserOperationClaimCommand, CreateUserOperationClaimCommandResponse>
    {
        private readonly IUserOperationClaimRepository userOperationClaimRepository;
        private readonly UserOperationClaimBusinessRules userOperationClaimBusinessRules;
        private readonly UserBusinessRules userBusinessRules;
        private readonly OperationClaimBusinessRules operationClaimBusinessRules;

        public CreateUserOperationClaimCommandHandler(
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

        public async Task<CreateUserOperationClaimCommandResponse> Handle(
            CreateUserOperationClaimCommand request,
            CancellationToken cancellationToken)
        {
            await operationClaimBusinessRules.OperationClaimIdShouldBeExistsWhenSelected(request.OperationClaimId);
            await userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);
            await userOperationClaimBusinessRules.UserShouldNotHaveOperationClaimAlreadyWhenInsert(request.UserId, request.OperationClaimId);

            var userOpClaim = request.Adapt<UserOperationClaimEntity>();
            userOpClaim.Id = Guid.NewGuid();

            await userOperationClaimRepository.AddAsync(userOpClaim, cancellationToken);

            var createdWithNavigation = await userOperationClaimRepository.GetAsync(
                predicate: x => x.Id == userOpClaim.Id,
                include: x => x.Include(u => u.User)
                               .Include(u => u.OperationClaim),
                cancellationToken: cancellationToken
            );

            return new CreateUserOperationClaimCommandResponse
            {
                Id = createdWithNavigation!.Id,
                UserId = createdWithNavigation.UserId,
                OperationClaimId = createdWithNavigation.OperationClaimId,
                OperationClaimName = createdWithNavigation.OperationClaim.Name,
                UserFullName = $"{createdWithNavigation.User.FirstName} {createdWithNavigation.User.LastName}"
            };
        }
    }
}

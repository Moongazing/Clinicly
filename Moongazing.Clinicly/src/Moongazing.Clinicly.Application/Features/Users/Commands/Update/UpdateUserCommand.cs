using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Features.Users.Constants;
using Moongazing.Clinicly.Application.Features.Users.Rules;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Clinicly.Domain.Enums;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Pipelines.Transaction;
using Moongazing.Kernel.Security.Constants;
using Moongazing.Kernel.Security.Hashing;

namespace Moongazing.Clinicly.Application.Features.Users.Commands.Update;

public class UpdateUserCommand : IRequest<UpdateUserResponse>,
    ILoggableRequest,
    IIntervalRequest,
    ITransactionalRequest,
    ICacheRemoverRequest,
    ISecuredRequest,
    IRateLimitedRequest
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public UserStatus Status { get; set; } = default!;
    public string[] Roles => [GeneralOperationClaims.Admin, GeneralOperationClaims.Update, GeneralOperationClaims.Write];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UsersMessages.SectionName;
    public string? CacheKey => null;
    public int Interval => 15;
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
    {
        private readonly IUserRepository userRepository;
        private readonly UserBusinessRules userBusinessRules;

        public UpdateUserCommandHandler(IUserRepository userRepository, UserBusinessRules userBusinessRules)
        {
            this.userRepository = userRepository;
            this.userBusinessRules = userBusinessRules;
        }

        public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {


            UserEntity? existingUser = await userRepository.GetAsync(predicate: u => u.Id == request.Id,
                                                                     withDeleted: false,
                                                                     cancellationToken: cancellationToken);

            await userBusinessRules.UserShouldBeExistsWhenSelected(existingUser);

            request.Adapt(existingUser);

            HashingHelper.CreateHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            existingUser!.PasswordHash = passwordHash;
            existingUser!.PasswordSalt = passwordSalt;

            UserEntity updatedUser = await userRepository.UpdateAsync(existingUser, cancellationToken);

            UpdateUserResponse response = updatedUser.Adapt<UpdateUserResponse>();

            return response;
        }
    }
}

using Mapster;
using MapsterMapper;
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


namespace Moongazing.Clinicly.Application.Features.Users.Commands.Create;

public class CreateUserCommand : IRequest<CreateUserResponse>,
    ILoggableRequest,
    IIntervalRequest,
    ITransactionalRequest,
    ICacheRemoverRequest,
    IRateLimitedRequest
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; }  = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public UserStatus Status { get; set; } = default!;


    public string[] Roles => [GeneralOperationClaims.Admin, GeneralOperationClaims.Add, GeneralOperationClaims.Write];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UsersMessages.SectionName;
    public string? CacheKey => null;
    public int Interval => 15;
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IUserRepository userRepository;
        private readonly UserBusinessRules userBusinessRules;

        public CreateUserCommandHandler(IUserRepository userRepository,
                                        UserBusinessRules userBusinessRules)
        {
            this.userRepository = userRepository;
            this.userBusinessRules = userBusinessRules;
        }

        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            await userBusinessRules.UserEmailShouldNotExists(request.Email);

            UserEntity user = request.Adapt<UserEntity>();

            HashingHelper.CreateHash(request.Password,
                inputHash: out byte[] passwordHash,
                inputSalt: out byte[] passwordSalt);


            user.Id = Guid.NewGuid();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            UserEntity createdUser = await userRepository.AddAsync(user, cancellationToken);

            CreateUserResponse response = createdUser.Adapt<CreateUserResponse>();

            return response;
        }

    }
}

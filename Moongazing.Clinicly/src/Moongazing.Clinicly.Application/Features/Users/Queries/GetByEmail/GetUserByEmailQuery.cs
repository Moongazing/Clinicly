using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Features.Users.Constants;
using Moongazing.Clinicly.Application.Features.Users.Rules;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.Users.Queries.GetByEmail;

public class GetUserByEmailQuery : IRequest<GetUserByEmailResponse>,
    ILoggableRequest,
    IIntervalRequest,
    ICachableRequest,
    ISecuredRequest,
    IRateLimitedRequest
{
    public string Email { get; set; } = default!;
    public string[] Roles => [GeneralOperationClaims.Read];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UsersMessages.SectionName;
    public string CacheKey => $"{GetType().Name}({Email})";
    public TimeSpan? SlidingExpiration { get; }
    public int Interval => 15;


    public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, GetUserByEmailResponse>
    {
        private readonly IUserRepository userRepository;
        private readonly UserBusinessRules userBusinessRules;

        public GetUserByEmailQueryHandler(IUserRepository userRepository, UserBusinessRules userBusinessRules)
        {
            this.userRepository = userRepository;
            this.userBusinessRules = userBusinessRules;
        }

        public async Task<GetUserByEmailResponse> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            UserEntity? user = await userRepository.GetAsync(predicate: u => u.Email == request.Email,
                                                             withDeleted: false,
                                                             cancellationToken: cancellationToken);

            await userBusinessRules.UserShouldBeExistsWhenSelected(user);


            GetUserByEmailResponse response = user.Adapt<GetUserByEmailResponse>();

            return response;
        }
    }
}

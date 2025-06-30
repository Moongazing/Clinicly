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

namespace Moongazing.Clinicly.Application.Features.Users.Queries.GetById;

public class GetUserByIdQuery : IRequest<GetUserByIdResponse>,
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


    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
    {
        private readonly IUserRepository userRepository;
        private readonly UserBusinessRules userBusinessRules;
        public GetUserByIdQueryHandler(IUserRepository userRepository, UserBusinessRules userBusinessRules)
        {
            this.userRepository = userRepository;
            this.userBusinessRules = userBusinessRules;
        }
        public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            UserEntity? user = await userRepository.GetAsync(predicate: u => u.Id == request.Id,
                                                             withDeleted: false,
                                                             cancellationToken: cancellationToken);

            await userBusinessRules.UserShouldBeExistsWhenSelected(user);

            GetUserByIdResponse response = user.Adapt<GetUserByIdResponse>();

            return response;
        }
    }
}

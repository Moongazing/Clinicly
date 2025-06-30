using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Features.Users.Constants;
using Moongazing.Clinicly.Application.Features.Users.Rules;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Enums;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Responses;
using Moongazing.Kernel.Security.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moongazing.Clinicly.Application.Features.Users.Queries.GetByStatus;

public class GetUserByStatusQuery : IRequest<PaginatedResponse<GetUserByStatusResponse>>,
    ILoggableRequest,
    IIntervalRequest,
    ICachableRequest,
    IRateLimitedRequest
{
    public UserStatus Status { get; set; } = default!;
    public string[] Roles => [GeneralOperationClaims.Read];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UsersMessages.SectionName;
    public string CacheKey => $"{GetType().Name}({Status})";
    public TimeSpan? SlidingExpiration { get; }
    public int Interval => 15;

    public class GetUserByStatusQueryHandler : IRequestHandler<GetUserByStatusQuery, PaginatedResponse<GetUserByStatusResponse>>
    {
        private readonly IUserRepository userRepository;
        private readonly UserBusinessRules userBusinessRules;
        public GetUserByStatusQueryHandler(IUserRepository userRepository, UserBusinessRules userBusinessRules)
        {
            this.userRepository = userRepository;
            this.userBusinessRules = userBusinessRules;
        }
        public async Task<PaginatedResponse<GetUserByStatusResponse>> Handle(GetUserByStatusQuery request, CancellationToken cancellationToken)
        {
            var users = await userRepository.GetListAsync(predicate: u => u.Status == request.Status,
                                                          withDeleted: false,
                                                          cancellationToken: cancellationToken);

            var response = users.Adapt<PaginatedResponse<GetUserByStatusResponse>>();
            return response;
        }
    }

}

using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Features.Users.Constants;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Requests;
using Moongazing.Kernel.Application.Responses;
using Moongazing.Kernel.Persistence.Paging;
using Moongazing.Kernel.Security.Constants;

namespace Moongazing.Clinicly.Application.Features.Users.Queries.GetList;

public class GetUserListQuery : IRequest<PaginatedResponse<GetUserListResponse>>,
    ILoggableRequest,
    IIntervalRequest,
    ICachableRequest,
    IRateLimitedRequest,
    ISecuredRequest
{
    public PageRequest PageRequest { get; set; } = default!;
    public string[] Roles => [GeneralOperationClaims.Read];
    public bool BypassCache { get; }
    public string? CacheGroupKey => UsersMessages.SectionName;
    public string CacheKey => $"{GetType().Name}({PageRequest.PageIndex}-{PageRequest.PageSize})";
    public TimeSpan? SlidingExpiration { get; }
    public int Interval => 15;


    public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, PaginatedResponse<GetUserListResponse>>
    {
        private readonly IUserRepository userRepository;

        public GetUserListQueryHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<PaginatedResponse<GetUserListResponse>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {
            IPaginate<UserEntity> users = await userRepository.GetListAsync(index: request.PageRequest.PageIndex,
                                                                            size: request.PageRequest.PageSize,
                                                                            withDeleted: false,
                                                                            cancellationToken: cancellationToken);

            PaginatedResponse<GetUserListResponse> response = users.Adapt<PaginatedResponse<GetUserListResponse>>();

            return response;
        }
    }
}

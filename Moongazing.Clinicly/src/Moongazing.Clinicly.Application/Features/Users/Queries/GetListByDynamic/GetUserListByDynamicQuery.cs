using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Requests;
using Moongazing.Kernel.Application.Responses;
using Moongazing.Kernel.Persistence.Dynamic;
using Moongazing.Kernel.Persistence.Paging;
using Moongazing.Kernel.Security.Constants;


namespace Moongazing.Clinicly.Application.Features.Users.Queries.GetListByDynamic;

public class GetUserListByDynamicQuery : IRequest<PaginatedResponse<GetUserListByDynamicResponse>>,
    ILoggableRequest,
    ISecuredRequest,
    IIntervalRequest, 
    IRateLimitedRequest
{
    public DynamicQuery DynamicQuery { get; set; } = default!;
    public PageRequest PageRequest { get; set; } = default!;
    public string[] Roles =>  [GeneralOperationClaims.Read];
    public int Interval => 15;

    public class GetUserListByDynamicQueryHandler : IRequestHandler<GetUserListByDynamicQuery, PaginatedResponse<GetUserListByDynamicResponse>>
    {
        private readonly IUserRepository userRepository;

        public GetUserListByDynamicQueryHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<PaginatedResponse<GetUserListByDynamicResponse>> Handle(GetUserListByDynamicQuery request, CancellationToken cancellationToken)
        {
            IPaginate<UserEntity> searchUsers = await userRepository.GetListByDynamicAsync(
                dynamic: request.DynamicQuery,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                withDeleted: false,
                cancellationToken: cancellationToken);

            PaginatedResponse<GetUserListByDynamicResponse> response = searchUsers.Adapt<PaginatedResponse<GetUserListByDynamicResponse>>();

            return response;
        }
    }
}

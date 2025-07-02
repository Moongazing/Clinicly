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

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetListByDynamic;

public class GetOperationClaimListByDynamicQuery : IRequest<PaginatedResponse<GetOperationClaimListByDynamicResponse>>,
    ILoggableRequest,
    ISecuredRequest,
    IIntervalRequest,
    IRateLimitedRequest
{
    public DynamicQuery DynamicQuery { get; set; } = default!;
    public PageRequest PageRequest { get; set; } = default!;
    public string[] Roles => [GeneralOperationClaims.Read];
    public int Interval => 15;

    public class GetOperationClaimListByDynamicQueryHandler : IRequestHandler<GetOperationClaimListByDynamicQuery, PaginatedResponse<GetOperationClaimListByDynamicResponse>>
    {
        private readonly IOperationClaimRepository operationClaimRepository;
        public GetOperationClaimListByDynamicQueryHandler(IOperationClaimRepository operationClaimRepository)
        {
            this.operationClaimRepository = operationClaimRepository;
        }
        public async Task<PaginatedResponse<GetOperationClaimListByDynamicResponse>> Handle(GetOperationClaimListByDynamicQuery request, CancellationToken cancellationToken)
        {
            IPaginate<OperationClaimEntity> searchOperationClaims = await operationClaimRepository.GetListByDynamicAsync(
                dynamic: request.DynamicQuery,
                index: request.PageRequest.PageIndex,
                size: request.PageRequest.PageSize,
                withDeleted: false,
                cancellationToken: cancellationToken);

            PaginatedResponse<GetOperationClaimListByDynamicResponse> response = searchOperationClaims.Adapt<PaginatedResponse<GetOperationClaimListByDynamicResponse>>();
            return response;
        }
    }
}

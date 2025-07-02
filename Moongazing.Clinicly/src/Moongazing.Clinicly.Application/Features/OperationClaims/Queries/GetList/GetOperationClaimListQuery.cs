using Mapster;
using MediatR;
using Moongazing.Clinicly.Application.Features.OperationClaims.Constants;
using Moongazing.Clinicly.Application.Features.Users.Constants;
using Moongazing.Clinicly.Application.Features.Users.Queries.GetList;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetList;

public class GetOperationClaimListQuery : IRequest<PaginatedResponse<GetOperationClaimListResponse>>,
    ILoggableRequest,
    IIntervalRequest,
    ICachableRequest,
    IRateLimitedRequest,
    ISecuredRequest
{
    public PageRequest PageRequest { get; set; } = default!;
    public string[] Roles => [GeneralOperationClaims.Read];
    public bool BypassCache { get; }
    public string? CacheGroupKey => OperationClaimMessages.SectionName;
    public string CacheKey => $"{GetType().Name}({PageRequest.PageIndex}-{PageRequest.PageSize})";
    public TimeSpan? SlidingExpiration { get; }
    public int Interval => 15;


    public class GetOperationClaimListQueryHandler : IRequestHandler<GetOperationClaimListQuery, PaginatedResponse<GetOperationClaimListResponse>>
    {
        private readonly IOperationClaimRepository operationClaimRepository;

        public GetOperationClaimListQueryHandler(IOperationClaimRepository operationClaimRepository)
        {
            this.operationClaimRepository = operationClaimRepository;
        }

        public async Task<PaginatedResponse<GetOperationClaimListResponse>> Handle(GetOperationClaimListQuery request, CancellationToken cancellationToken)
        {
            IPaginate<OperationClaimEntity> operationClaims = await operationClaimRepository.GetListAsync(index: request.PageRequest.PageIndex,
                                                                                                          size: request.PageRequest.PageSize,
                                                                                                          withDeleted: false,
                                                                                                          cancellationToken: cancellationToken);

            PaginatedResponse<GetOperationClaimListResponse> response = operationClaims.Adapt<PaginatedResponse<GetOperationClaimListResponse>>();

            return response;
        }
    }
}

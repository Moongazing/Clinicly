using MediatR;
using Moongazing.Clinicly.Application.Features.Users.Constants;
using Moongazing.Clinicly.Domain.Enums;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.Performance;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Requests;
using Moongazing.Kernel.Application.Responses;
using Moongazing.Kernel.Security.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}

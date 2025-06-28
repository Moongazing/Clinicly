using MediatR;
using Microsoft.AspNetCore.Http;
using Moongazing.Kernel.CrossCuttingConcerns.Exceptions.Types;
using Moongazing.Kernel.Security.Constants;
using Moongazing.Kernel.Security.Extensions;

namespace Moongazing.Kernel.Application.Pipelines.Authorization;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor httpContextAccessor;
    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        ICollection<string>? userRoleClaims = httpContextAccessor.HttpContext?.User?.GetRoleClaims()
            ?? throw new AuthorizationException("Claims Not Found");

        bool isMatchedAUserRoleClaimWithRequestRoles = !userRoleClaims.Any(userRoleClaim =>
               userRoleClaim == GeneralOperationClaims.Admin || request.Roles.Contains(userRoleClaim));

        if (isMatchedAUserRoleClaimWithRequestRoles)
        {

            throw new AuthorizationException("Not Authorized");
        }

        TResponse response = await next(cancellationToken);

        return response;
    }
}

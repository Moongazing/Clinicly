using Moongazing.Kernel.Application.Responses;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetListByDynamic;

public class GetOperationClaimListByDynamicResponse:IResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

}

using Moongazing.Kernel.Application.Responses;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetList;

public class GetOperationClaimListResponse:IResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}
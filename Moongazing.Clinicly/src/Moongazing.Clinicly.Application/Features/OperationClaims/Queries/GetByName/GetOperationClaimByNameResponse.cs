using Moongazing.Kernel.Application.Responses;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetByName;

public class GetOperationClaimByNameResponse:IResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

}

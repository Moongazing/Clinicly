using Moongazing.Kernel.Application.Responses;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetById;

public class GetOperationClaimByIdResponse:IResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;

}

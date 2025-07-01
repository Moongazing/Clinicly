using Moongazing.Kernel.Application.Responses;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Commands.Delete;

public class DeleteOperationClaimResponse:IResponse
{
    public Guid Id { get; set; }
}
using Moongazing.Kernel.Application.Responses;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Commands.Delete;

public class DeleteUserOperationClaimCommandResponse : IResponse
{
    public Guid Id { get; set; }

}
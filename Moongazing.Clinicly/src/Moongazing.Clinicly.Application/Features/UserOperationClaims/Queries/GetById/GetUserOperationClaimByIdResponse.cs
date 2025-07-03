namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetById;

public class GetUserOperationClaimByIdResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OperationClaimId { get; set; }
    public string OperationClaimName { get; set; } = default!;
    public string UserFullName { get; set; } = default!;
}
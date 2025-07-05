namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Queries.GetByUserId;

public class GetUserOperationClaimsByUserIdResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OperationClaimId { get; set; }
    public string OperationClaimName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
}
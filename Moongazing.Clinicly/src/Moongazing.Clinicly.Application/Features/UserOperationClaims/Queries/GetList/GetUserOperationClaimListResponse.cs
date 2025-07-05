namespace Moongazing.Clinicly.Application.Features.OperationClaims.Queries.GetList;

public class GetUserOperationClaimListResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OperationClaimId { get; set; }
    public string OperationClaimName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
}
using Microsoft.EntityFrameworkCore;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Clinicly.Persistence.Context;
using Moongazing.Kernel.Persistence.Repositories;

namespace Moongazing.Clinicly.Persistence.Repositories;

public class UserOperationClaimRepository : EfRepositoryBase<UserOperationClaimEntity, Guid, BaseDbContext>, IUserOperationClaimRepository
{
    public UserOperationClaimRepository(BaseDbContext context) : base(context)
    {
    }
    public async Task<IList<OperationClaimEntity>> GetOperationClaimsByUserIdAsync(Guid userId)
    {
        var operationClaims = await Query()
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new OperationClaimEntity { Id = p.OperationClaimId, Name = p.OperationClaim.Name })
            .ToListAsync();
        return operationClaims;
    }
}

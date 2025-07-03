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
}

using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Persistence.Repositories;

namespace Moongazing.Clinicly.Application.Repositories;

public interface IUserOperationClaimRepository : IAsyncRepository<UserOperationClaimEntity, Guid>, IRepository<UserOperationClaimEntity, Guid>
{
}

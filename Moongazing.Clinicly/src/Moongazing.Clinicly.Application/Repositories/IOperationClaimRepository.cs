using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Persistence.Repositories;

namespace Moongazing.Clinicly.Application.Repositories;

public interface IOperationClaimRepository : IAsyncRepository<OperationClaimEntity, Guid>, IRepository<OperationClaimEntity, Guid>
{
}

using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Persistence.Repositories;

namespace Moongazing.Clinicly.Application.Repositories;

public interface IUserRepository : IAsyncRepository<UserEntity, Guid>, IRepository<UserEntity, Guid>
{
}

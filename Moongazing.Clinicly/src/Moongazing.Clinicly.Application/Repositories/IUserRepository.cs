using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moongazing.Clinicly.Application.Repositories;

public interface IUserRepository : IAsyncRepository<UserEntity, Guid>, IRepository<UserEntity, Guid>
{
}

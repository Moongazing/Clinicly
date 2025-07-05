using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Persistence.Repositories;

namespace Moongazing.Clinicly.Application.Repositories;

public interface IOtpAuthenticatorRepository : IAsyncRepository<OtpAuthenticatorEntity, Guid>, IRepository<OtpAuthenticatorEntity, Guid>
{
}

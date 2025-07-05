using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Persistence.Repositories;

namespace Moongazing.Clinicly.Application.Repositories;

public interface IEmailAuthenticatorRepository : IAsyncRepository<EmailAuthenticatorEntity, Guid>, IRepository<EmailAuthenticatorEntity, Guid>
{
}

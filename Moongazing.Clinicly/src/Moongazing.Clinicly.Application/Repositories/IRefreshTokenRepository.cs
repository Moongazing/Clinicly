using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Persistence.Repositories;

namespace Moongazing.Clinicly.Application.Repositories;

public interface IRefreshTokenRepository : IAsyncRepository<RefreshTokenEntity, Guid>, IRepository<RefreshTokenEntity, Guid>
{
    Task<List<RefreshTokenEntity>> GetOldRefreshTokensAsync(Guid userID, int refreshTokenTTL);
}

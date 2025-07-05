using Microsoft.EntityFrameworkCore;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Clinicly.Persistence.Context;
using Moongazing.Kernel.Persistence.Repositories;

namespace Moongazing.Clinicly.Persistence.Repositories;

public class RefreshTokenRepository : EfRepositoryBase<RefreshTokenEntity, Guid, BaseDbContext>, IRefreshTokenRepository
{
    public RefreshTokenRepository(BaseDbContext context) : base(context)
    {
    }

    public async Task<List<RefreshTokenEntity>> GetOldRefreshTokensAsync(Guid userID, int refreshTokenTTL)
    {
        List<RefreshTokenEntity> tokens = await Query()
            .AsNoTracking()
            .Where(r =>
                r.UserId == userID
                && r.Revoked == null
                && r.Expires >= DateTime.UtcNow
                && r.CreatedDate!.Value.AddDays(refreshTokenTTL) <= DateTime.UtcNow
            )
            .ToListAsync();

        return tokens;
    }
}
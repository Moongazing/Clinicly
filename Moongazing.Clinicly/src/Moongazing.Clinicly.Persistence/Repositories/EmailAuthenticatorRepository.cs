using Microsoft.EntityFrameworkCore;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Clinicly.Persistence.Context;
using Moongazing.Kernel.Persistence.Repositories;

namespace Moongazing.Clinicly.Persistence.Repositories;

public class EmailAuthenticatorRepository : EfRepositoryBase<EmailAuthenticatorEntity, Guid, BaseDbContext>, IEmailAuthenticatorRepository
{
    public EmailAuthenticatorRepository(BaseDbContext context) : base(context)
    {
    }

    public async Task<ICollection<EmailAuthenticatorEntity>> DeleteAllNonVerifiedAsync(UserEntity user)
    {
        var nonVerifiedAuthenticators = await Query()
                                      .Where(uea => uea.UserId == user.Id && !uea.IsVerified)
                                      .ToListAsync();

        if (nonVerifiedAuthenticators.Count != 0)
        {
            await DeleteRangeAsync(nonVerifiedAuthenticators);
        }

        return nonVerifiedAuthenticators;
    }
}
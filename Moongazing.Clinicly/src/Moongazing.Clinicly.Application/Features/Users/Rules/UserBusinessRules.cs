using Moongazing.Clinicly.Application.Features.Users.Constants;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Rules;
using Moongazing.Kernel.CrossCuttingConcerns.Exceptions.Types;
using Moongazing.Kernel.Localization.Abstractions;
using Moongazing.Kernel.Security.Hashing;

namespace Moongazing.Clinicly.Application.Features.Users.Rules;

public class UserBusinessRules : BaseBusinessRules
{
    private readonly IUserRepository userRepository;
    private readonly ILocalizationService localizationService;

    public UserBusinessRules(IUserRepository userRepository, ILocalizationService localizationService)
    {
        this.userRepository = userRepository;
        this.localizationService = localizationService;
    }

    private async Task LocalizedBusinessException(string messageKey)
    {
        var message = await localizationService.GetLocalizedAsync(messageKey, UsersMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task UserShouldBeActive(UserEntity? user)
    {

        if (user!.Status == Domain.Enums.UserStatus.Inactive)
        {
            await LocalizedBusinessException(UsersMessages.UserNotActive);
        }
    }

    public async Task UserShouldNotSuspended(UserEntity? user)
    {
        if (user!.Status == Domain.Enums.UserStatus.Suspended)
        {
            await LocalizedBusinessException(UsersMessages.UserSuspended);
        }
    }
    public async Task UserShouldBeExistsWhenSelected(UserEntity? user)
    {
        if (user == null)
        {
            await LocalizedBusinessException(UsersMessages.UserNotFound);
        }
    }

    public async Task UserIdShouldBeExistsWhenSelected(Guid id)
    {
        bool doesExist = await userRepository.AnyAsync(predicate: u => u.Id == id);
        if (!doesExist)
        {
            await LocalizedBusinessException(UsersMessages.UserNotFound);
        }
    }

    public async Task UserPasswordShouldBeMatched(UserEntity user, string password)
    {
        if (!HashingHelper.VerifyHash(password, user.PasswordHash, user.PasswordSalt))
        {
            await LocalizedBusinessException(UsersMessages.PasswordMismatch);
        }
    }

    public async Task UserEmailShouldNotExists(string email)
    {
        bool doesExists = await userRepository.AnyAsync(predicate: u => u.Email == email);
        if (doesExists)
        {
            await LocalizedBusinessException(UsersMessages.EmailAlreadyExists);
        }
    }

  
}

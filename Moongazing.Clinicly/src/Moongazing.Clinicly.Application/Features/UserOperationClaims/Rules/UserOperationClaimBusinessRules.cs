using Moongazing.Clinicly.Application.Features.UserOperationClaims.Constants;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Rules;
using Moongazing.Kernel.CrossCuttingConcerns.Exceptions.Types;
using Moongazing.Kernel.Localization.Abstractions;

namespace Moongazing.Clinicly.Application.Features.UserOperationClaims.Rules;

public class UserOperationClaimBusinessRules : BaseBusinessRules
{
    private readonly IUserOperationClaimRepository userOperationClaimRepository;
    private readonly ILocalizationService localizationService;

    public UserOperationClaimBusinessRules(IUserOperationClaimRepository userOperationClaimRepository,
                                           ILocalizationService localizationService)
    {
        this.userOperationClaimRepository = userOperationClaimRepository;
        this.localizationService = localizationService;
    }

    private async Task LocalizedBusinessException(string messageKey)
    {
        var message = await localizationService.GetLocalizedAsync(messageKey, UserOperationClaimsMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task UserOperationClaimShouldExistWhenSelected(UserOperationClaimEntity? userOperationClaim)
    {
        if (userOperationClaim == null)
        {
            await LocalizedBusinessException(UserOperationClaimsMessages.UserOperationClaimNotFound);
        }
    }

    public async Task UserOperationClaimIdShouldExistWhenSelected(Guid id)
    {
        bool doesExist = await userOperationClaimRepository.AnyAsync(predicate: b => b.Id == id);
        if (!doesExist)
        {
            await LocalizedBusinessException(UserOperationClaimsMessages.UserOperationClaimNotFound);
        }
    }
    public async Task UserShouldNotHaveOperationClaimAlreadyWhenInsert(Guid userId, Guid operationClaimId)
    {
        bool doesExist = await userOperationClaimRepository.AnyAsync(u => u.UserId == userId && u.OperationClaimId == operationClaimId);
        if (doesExist)
        {
            await LocalizedBusinessException(UserOperationClaimsMessages.UserOperationClaimAlreadyExists);
        }
    }
}
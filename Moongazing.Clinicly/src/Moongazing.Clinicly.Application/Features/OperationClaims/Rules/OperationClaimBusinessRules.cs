using Moongazing.Clinicly.Application.Features.OperationClaims.Constants;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Domain.Entities;
using Moongazing.Kernel.Application.Rules;
using Moongazing.Kernel.CrossCuttingConcerns.Exceptions.Types;
using Moongazing.Kernel.Localization.Abstractions;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Rules;

public class OperationClaimBusinessRules : BaseBusinessRules
{
    private readonly IOperationClaimRepository operationClaimRepository;
    private readonly ILocalizationService localizationService;

    public OperationClaimBusinessRules(IOperationClaimRepository operationClaimRepository,
                                       ILocalizationService localizationService)
    {
        this.operationClaimRepository = operationClaimRepository;
        this.localizationService = localizationService;
    }

    private async Task LocalizedBusinessException(string messageKey)
    {
        var message = await localizationService.GetLocalizedAsync(messageKey, OperationClaimMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task OperationClaimShouldBeExistsWhenSelected(OperationClaimEntity? operationClaim)
    {
        if (operationClaim is null)
        {
            await LocalizedBusinessException(OperationClaimMessages.OperationClaimNotFound);
        }
    }

    public async Task OperationClaimIdShouldBeExistsWhenSelected(Guid id)
    {
        bool exists = await operationClaimRepository.AnyAsync(oc => oc.Id == id);
        if (!exists)
            await LocalizedBusinessException(OperationClaimMessages.OperationClaimNotFound);
    }

    public async Task OperationClaimNameShouldNotExists(string name)
    {
        bool exists = await operationClaimRepository.AnyAsync(oc => oc.Name == name);
        if (exists)
            await LocalizedBusinessException(OperationClaimMessages.OperationClaimAlreadyExists);
    }


}

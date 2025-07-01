using FluentValidation;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Commands.Delete;

public class DeleteOperationClaimCommandValidator : AbstractValidator<DeleteOperationClaimCommand>
{
    public DeleteOperationClaimCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
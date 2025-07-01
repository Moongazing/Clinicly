using FluentValidation;

namespace Moongazing.Clinicly.Application.Features.OperationClaims.Commands.Update;

public class UpdateOpertionClaimCommandValidator : AbstractValidator<UpdateOperationClaimCommand>
{
    public UpdateOpertionClaimCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100);
    }
}
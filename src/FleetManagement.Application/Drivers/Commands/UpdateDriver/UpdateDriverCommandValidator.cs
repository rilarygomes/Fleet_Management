using FluentValidation;

namespace FleetManagement.Application.Drivers.Commands.UpdateDriver;

public class UpdateDriverCommandValidator : AbstractValidator<UpdateDriverCommand>
{
    public UpdateDriverCommandValidator()
    {
        RuleFor(d => d.Name)
            .NotEmpty()
            .WithMessage("Driver name is required.")
            .MinimumLength(3)
            .WithMessage("Driver name must have at least 3 characters.");

        RuleFor(d => d.LicenseNumber)
            .NotEmpty()
            .WithMessage("License number is required.")
            .Matches(@"^\d{11}$")
            .WithMessage("License number must contain exactly 11 digits.");

        RuleFor(d => d.LicenseExpirationDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("License expiration date cannot be in the past.");
    }
}
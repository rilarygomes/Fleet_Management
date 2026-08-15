using FluentValidation;
using FleetManagement.Application.DTOs;

public class UpdateDriverValidator : AbstractValidator<UpdateDriverDto>
{
    public UpdateDriverValidator()
    {
        RuleFor(d => d.Name)
            .NotEmpty().WithMessage("Driver name is required.")
            .MinimumLength(3).WithMessage("Driver name must have at least 3 characters.");

        RuleFor(d => d.LicenseNumber)
            .NotEmpty().WithMessage("License number is required.")
            .Length(11).WithMessage("License number must be exactly 11 characters.");

        RuleFor(d => d.LicenseExpirationDate)
            .GreaterThanOrEqualTo(DateTime.Now)
            .WithMessage("License expiration date cannot be in the past.");
    }
}

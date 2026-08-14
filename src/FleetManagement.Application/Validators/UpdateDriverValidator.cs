using FluentValidation;
using FleetManagement.Application.DTOs;

public class UpdateDriverValidator : AbstractValidator<UpdateDriverDto>
{
    public UpdateDriverValidator()
    {
        RuleFor(d => d.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(d => d.LicenseNumber)
            .NotEmpty().WithMessage("License number is required.");

        RuleFor(d => d.LicenseExpirationDate)
            .GreaterThanOrEqualTo(DateTime.Now)
            .WithMessage("License expiration date cannot be in the past.");
    }
}

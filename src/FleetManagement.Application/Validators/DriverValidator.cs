using FluentValidation;
using FleetManagement.Application.DTOs;

namespace FleetManagement.Application.Validators
{
    public class DriverValidator : AbstractValidator<DriverDto>
    {
        public DriverValidator()
        {
            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Driver name is required")
                .MinimumLength(3).WithMessage("Driver name must have at least 3 characters");

            RuleFor(d => d.LicenseNumber)
                .NotEmpty().WithMessage("License number is required")
                .Length(11).WithMessage("License number must be 11 characters");

            RuleFor(d => d.LicenseExpirationDate)
            .GreaterThan(DateTime.Today).WithMessage("License expiration date must be in the future");
        }
    }
}

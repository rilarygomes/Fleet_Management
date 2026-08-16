using FluentValidation;
using FleetManagement.Application.Drivers.DTOs;

namespace FleetManagement.Application.Driver.Validators
{
    public class DriverValidator : AbstractValidator<DriverDto>
    {
        public DriverValidator()
        {
            RuleFor(d => d.Id)
                .NotEmpty().WithMessage("Id is required.")
                .Must(id => Guid.TryParse(id.ToString(), out _))
                .WithMessage("Id value didn’t follow the GUID model expected.");

            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("Driver name is required.")
                .MinimumLength(3).WithMessage("Driver name must have at least 3 characters.");

            RuleFor(d => d.LicenseNumber)
                .NotEmpty().WithMessage("License number is required.")
                .Length(11).WithMessage("License number must be exactly 11 characters.");

            RuleFor(d => d.LicenseExpirationDate)
                .GreaterThan(DateTime.Today)
                .WithMessage("License expiration date must be in the future.");
        }
    }
}

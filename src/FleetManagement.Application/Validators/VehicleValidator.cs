using FluentValidation;
using FleetManagement.Application.DTOs;

namespace FleetManagement.Application.Validators
{
    public class VehicleValidator : AbstractValidator<VehicleDto>
    {
        public VehicleValidator()
        {
            RuleFor(v => v.LicensePlate)
                .NotEmpty().WithMessage("License plate is required")
                .Length(7).WithMessage("License plate must be 7 characters");

            RuleFor(v => v.Model)
                .NotEmpty().WithMessage("Model is required");

            RuleFor(v => v.Year)
                .InclusiveBetween(1980, DateTime.Now.Year)
                .WithMessage("Year must be valid");
        }
    }
}

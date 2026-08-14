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
                .NotEmpty().WithMessage("Model is required")
                .MinimumLength(2).WithMessage("Model must have at least 2 characters");

            RuleFor(v => v.Year)
                .InclusiveBetween(1960, DateTime.Now.Year + 1)
                .WithMessage("Year must be between 1960 and next year");
        }
    }
}

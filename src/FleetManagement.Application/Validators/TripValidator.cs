using FluentValidation;
using FleetManagement.Application.DTOs;

namespace FleetManagement.Application.Validators
{
    public class TripValidator : AbstractValidator<TripDto>
    {
        public TripValidator()
        {
            RuleFor(t => t.VehicleId)
                .NotEmpty().WithMessage("VehicleId is required");

            RuleFor(t => t.DriverId)
                .NotEmpty().WithMessage("DriverId is required");

            RuleFor(t => t.StartDate)
                .LessThanOrEqualTo(t => t.EndDate)
                .WithMessage("StartDate must be before or equal to EndDate");

            RuleFor(t => t.EndDate)
                .GreaterThanOrEqualTo(t => t.StartDate)
                .WithMessage("EndDate must be after or equal to StartDate");

            RuleFor(t => t.StartDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("StartDate cannot be in the past");
        }
    }
}

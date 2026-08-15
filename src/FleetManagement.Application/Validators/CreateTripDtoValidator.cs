using FluentValidation;
using FleetManagement.Application.DTOs;

namespace FleetManagement.Application.Validators
{
    public class CreateTripDtoValidator : AbstractValidator<CreateTripDto>
    {
        public CreateTripDtoValidator()
        {
            RuleFor(t => t.VehicleId)
                .NotEmpty().WithMessage("VehicleId is required.")
                .Must(id => Guid.TryParse(id.ToString(), out _))
                .WithMessage("VehicleId value didn’t follow the GUID model expected.");

            RuleFor(t => t.DriverId)
                .NotEmpty().WithMessage("DriverId is required.")
                .Must(id => Guid.TryParse(id.ToString(), out _))
                .WithMessage("DriverId value didn’t follow the GUID model expected.");

            RuleFor(t => t.StartDate)
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("StartDate cannot be in the past.")
                .LessThanOrEqualTo(t => t.EndDate)
                .WithMessage("StartDate must be before or equal to EndDate.");

            RuleFor(t => t.EndDate)
                .GreaterThanOrEqualTo(t => t.StartDate)
                .WithMessage("EndDate must be after or equal to StartDate.");
        }
    }
}

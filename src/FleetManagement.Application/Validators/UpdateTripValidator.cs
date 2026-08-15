using FluentValidation;
using FleetManagement.Application.DTOs;

public class UpdateTripValidator : AbstractValidator<UpdateTripDto>
{
    public UpdateTripValidator()
    {
        RuleFor(t => t.VehicleId)
            .NotEmpty().WithMessage("VehicleId is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _))
            .WithMessage("VehicleId must be a valid GUID.");

        RuleFor(t => t.DriverId)
            .NotEmpty().WithMessage("DriverId is required.")
            .Must(id => Guid.TryParse(id.ToString(), out _))
            .WithMessage("DriverId must be a valid GUID.");

        RuleFor(t => t.StartDate)
            .LessThanOrEqualTo(t => t.EndDate)
            .WithMessage("StartDate must be before or equal to EndDate.");

        RuleFor(t => t.EndDate)
            .GreaterThanOrEqualTo(t => t.StartDate)
            .WithMessage("EndDate must be after or equal to StartDate.");
    }
}

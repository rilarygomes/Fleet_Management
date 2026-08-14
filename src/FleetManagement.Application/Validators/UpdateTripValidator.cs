using FluentValidation;
using FleetManagement.Application.DTOs;

public class UpdateTripValidator : AbstractValidator<UpdateTripDto>
{
    public UpdateTripValidator()
    {
        RuleFor(t => t.VehicleId)
            .NotEmpty().WithMessage("VehicleId is required.");

        RuleFor(t => t.DriverId)
            .NotEmpty().WithMessage("DriverId is required.");

        RuleFor(t => t.StartDate)
            .LessThanOrEqualTo(t => t.EndDate)
            .WithMessage("StartDate must be before or equal to EndDate.");

        RuleFor(t => t.EndDate)
            .GreaterThanOrEqualTo(t => t.StartDate)
            .WithMessage("EndDate must be after or equal to StartDate.");

        RuleFor(t => t).Custom((dto, context) =>
        {
            var existing = context.RootContextData["ExistingTrip"] as TripDto;
            if (existing != null && existing.StartDate <= DateTime.Now)
            {
                if (dto.VehicleId != existing.VehicleId ||
                    dto.DriverId != existing.DriverId ||
                    dto.StartDate != existing.StartDate)
                {
                    context.AddFailure("Only EndDate can be updated for trips that have already started.");
                }
            }
        });
    }
}

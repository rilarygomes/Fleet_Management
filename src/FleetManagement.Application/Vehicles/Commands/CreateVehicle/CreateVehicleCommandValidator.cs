using FluentValidation;

namespace FleetManagement.Application.Vehicles.Commands.CreateVehicle;

public class CreateVehicleCommandValidator
    : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(v => v.LicensePlate)
            .NotEmpty()
            .WithMessage("License plate is required.")
            .Length(7)
            .WithMessage("License plate must be exactly 7 characters.");

        RuleFor(v => v.Model)
            .NotEmpty()
            .WithMessage("Model is required.")
            .MinimumLength(2)
            .WithMessage("Model must have at least 2 characters.");

        RuleFor(v => v.Year)
            .InclusiveBetween(1960, DateTime.Now.Year)
            .WithMessage("Year must be between 1960 and the current year.");
    }
}
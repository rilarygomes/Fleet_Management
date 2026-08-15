using FluentValidation;
using FleetManagement.Application.DTOs;

public class UpdateVehicleValidator : AbstractValidator<UpdateVehicleDto>
{
    public UpdateVehicleValidator()
    {
        RuleFor(v => v.LicensePlate)
            .NotEmpty().WithMessage("License plate is required.")
            .Length(7).WithMessage("License plate must be exactly 7 characters.");

        RuleFor(v => v.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MinimumLength(2).WithMessage("Model must have at least 2 characters.");

        RuleFor(v => v.Year)
            .LessThanOrEqualTo(DateTime.Now.Year)
            .WithMessage("Manufacturing year cannot be in the future.");
    }
}

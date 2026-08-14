using FluentValidation;
using FleetManagement.Application.DTOs;

public class UpdateVehicleValidator : AbstractValidator<UpdateVehicleDto>
{
    public UpdateVehicleValidator()
    {
        RuleFor(v => v.LicensePlate)
            .NotEmpty().WithMessage("License plate is required.");

        RuleFor(v => v.Model)
            .NotEmpty().WithMessage("Model is required.");

        RuleFor(v => v.Year)
            .LessThanOrEqualTo(DateTime.Now.Year)
            .WithMessage("Manufacturing year cannot be in the future.");
    }
}

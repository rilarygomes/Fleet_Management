using FleetManagement.Application.Shared;
using FleetManagement.Application.Vehicles.DTOs;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Vehicles.Commands.UpdateVehicle;

public class UpdateVehicleCommandHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IValidator<UpdateVehicleCommand> _validator;
    private readonly ILogger<UpdateVehicleCommandHandler> _logger;

    public UpdateVehicleCommandHandler(
        IVehicleRepository vehicleRepository,
        IValidator<UpdateVehicleCommand> validator,
        ILogger<UpdateVehicleCommandHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _validator = validator;
        _logger = logger;
    }

    public Result<VehicleDto> Handle(Guid id, UpdateVehicleCommand command)
    {
        _logger.LogInformation(
            "Updating vehicle {VehicleId}",
            id);

        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                "; ",
                validationResult.Errors.Select(error => error.ErrorMessage));

            _logger.LogWarning(
                "Vehicle {VehicleId} validation failed: {ValidationErrors}",
                id,
                errors);

            return Result<VehicleDto>.Fail(errors);
        }

        var vehicle = _vehicleRepository.GetById(id);

        if (vehicle is null)
        {
            _logger.LogWarning(
                "Vehicle {VehicleId} not found",
                id);

            return Result<VehicleDto>.Fail("Vehicle not found.");
        }

        var duplicate = _vehicleRepository
            .GetByLicensePlate(command.LicensePlate);

        if (duplicate is not null && duplicate.Id != id)
        {
            _logger.LogWarning(
                "License plate {LicensePlate} already belongs to another vehicle",
                command.LicensePlate);

            return Result<VehicleDto>.Fail(
                "Another vehicle already uses this license plate.");
        }

        vehicle.Update(
            command.LicensePlate,
            command.Model,
            command.Year);

        _vehicleRepository.Update(vehicle);
        _vehicleRepository.SaveChanges();

        _logger.LogInformation(
            "Vehicle {VehicleId} updated successfully",
            id);

        return Result<VehicleDto>.Ok(new VehicleDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Model = vehicle.Model,
            Year = vehicle.Year
        });
    }
}
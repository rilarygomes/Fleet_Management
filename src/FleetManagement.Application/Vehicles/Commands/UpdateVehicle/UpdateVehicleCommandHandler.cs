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

    public Result<VehicleDto> Handle(UpdateVehicleCommand command)
    {
        _logger.LogInformation(
            "Updating vehicle {VehicleId}",
            command.Id);

        _validator.ValidateAndThrow(command);

        var vehicle = _vehicleRepository.GetById(command.Id);

        if (vehicle == null)
        {
            _logger.LogWarning(
                "Vehicle {VehicleId} not found.",
                command.Id);

            return Result<VehicleDto>.Fail("Vehicle not found.");
        }

        var duplicate = _vehicleRepository.GetByLicensePlate(command.LicensePlate);

        if (duplicate != null && duplicate.Id != command.Id)
        {
            _logger.LogWarning(
                "License plate {LicensePlate} already belongs to another vehicle.",
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
            "Vehicle {VehicleId} updated successfully.",
            vehicle.Id);

        return Result<VehicleDto>.Ok(new VehicleDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Model = vehicle.Model,
            Year = vehicle.Year
        });
    }
}
using FleetManagement.Application.Shared;
using FleetManagement.Application.Vehicles.DTOs;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Vehicles.Commands.CreateVehicle;

public class CreateVehicleCommandHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IValidator<CreateVehicleCommand> _validator;
    private readonly ILogger<CreateVehicleCommandHandler> _logger;

    public CreateVehicleCommandHandler(
        IVehicleRepository vehicleRepository,
        IValidator<CreateVehicleCommand> validator,
        ILogger<CreateVehicleCommandHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _validator = validator;
        _logger = logger;
    }

    public Result<VehicleDto> Handle(CreateVehicleCommand command)
    {
        _logger.LogInformation(
            "Creating vehicle with LicensePlate {LicensePlate}",
            command.LicensePlate);

        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                "; ",
                validationResult.Errors.Select(error => error.ErrorMessage));

            _logger.LogWarning(
                "Vehicle creation validation failed: {ValidationErrors}",
                errors);

            return Result<VehicleDto>.Fail(errors);
        }

        var existing = _vehicleRepository
            .GetByLicensePlate(command.LicensePlate);

        if (existing is not null)
        {
            _logger.LogWarning(
                "Vehicle with LicensePlate {LicensePlate} already exists",
                command.LicensePlate);

            return Result<VehicleDto>.Fail(
                "A vehicle with this license plate already exists.");
        }

        var vehicle = new Vehicle(
            Guid.NewGuid(),
            command.LicensePlate,
            command.Model,
            command.Year);

        _vehicleRepository.Add(vehicle);
        _vehicleRepository.SaveChanges();

        _logger.LogInformation(
            "Vehicle {VehicleId} created successfully",
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
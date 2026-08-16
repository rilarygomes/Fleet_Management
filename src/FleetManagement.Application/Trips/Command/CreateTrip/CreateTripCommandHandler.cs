using FleetManagement.Application.Shared;
using FleetManagement.Application.Trips.DTOs;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Trips.Commands.CreateTrip;

public class CreateTripCommandHandler
{
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IValidator<CreateTripCommand> _validator;
    private readonly ILogger<CreateTripCommandHandler> _logger;

    public CreateTripCommandHandler(
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository,
        IDriverRepository driverRepository,
        IValidator<CreateTripCommand> validator,
        ILogger<CreateTripCommandHandler> logger)
    {
        _tripRepository = tripRepository;
        _vehicleRepository = vehicleRepository;
        _driverRepository = driverRepository;
        _validator = validator;
        _logger = logger;
    }

    public Result<TripDto> Handle(CreateTripCommand command)
    {
        _logger.LogInformation(
            "Creating trip for Vehicle {VehicleId} and Driver {DriverId}",
            command.VehicleId,
            command.DriverId);

        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                "; ",
                validationResult.Errors.Select(error => error.ErrorMessage));

            _logger.LogWarning(
                "Trip creation validation failed: {ValidationErrors}",
                errors);

            return Result<TripDto>.Fail(errors);
        }

        var vehicle = _vehicleRepository.GetById(command.VehicleId);

        if (vehicle is null)
        {
            _logger.LogWarning(
                "Vehicle {VehicleId} not found",
                command.VehicleId);

            return Result<TripDto>.Fail("Vehicle not found.");
        }

        var driver = _driverRepository.GetById(command.DriverId);

        if (driver is null)
        {
            _logger.LogWarning(
                "Driver {DriverId} not found",
                command.DriverId);

            return Result<TripDto>.Fail("Driver not found.");
        }

        var conflictingVehicleTrip = _tripRepository
            .GetTripsByVehicle(command.VehicleId)
            .FirstOrDefault(t =>
                t.StartDate < command.EndDate &&
                command.StartDate < t.EndDate);

        if (conflictingVehicleTrip is not null)
        {
            _logger.LogWarning(
                "Vehicle {VehicleId} has a conflicting trip {TripId}",
                command.VehicleId,
                conflictingVehicleTrip.Id);

            return Result<TripDto>.Fail(
                $"Vehicle is already assigned to another trip from " +
                $"{conflictingVehicleTrip.StartDate} to " +
                $"{conflictingVehicleTrip.EndDate}.");
        }

        var conflictingDriverTrip = _tripRepository
            .GetTripsByDriver(command.DriverId)
            .FirstOrDefault(t =>
                t.StartDate < command.EndDate &&
                command.StartDate < t.EndDate);

        if (conflictingDriverTrip is not null)
        {
            _logger.LogWarning(
                "Driver {DriverId} has a conflicting trip {TripId}",
                command.DriverId,
                conflictingDriverTrip.Id);

            return Result<TripDto>.Fail(
                $"Driver is already assigned to another trip from " +
                $"{conflictingDriverTrip.StartDate} to " +
                $"{conflictingDriverTrip.EndDate}.");
        }

        var trip = new Trip(
            Guid.NewGuid(),
            command.VehicleId,
            command.DriverId,
            command.StartDate,
            command.EndDate);

        _tripRepository.Add(trip);
        _tripRepository.SaveChanges();

        _logger.LogInformation(
            "Trip {TripId} created successfully",
            trip.Id);

        return Result<TripDto>.Ok(new TripDto
        {
            Id = trip.Id,
            VehicleId = trip.VehicleId,
            DriverId = trip.DriverId,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate
        });
    }
}
using FleetManagement.Application.Shared;
using FleetManagement.Application.Trips.DTOs;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Trips.Commands.UpdateTrip;

public class UpdateTripCommandHandler
{
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IValidator<UpdateTripCommand> _validator;
    private readonly ILogger<UpdateTripCommandHandler> _logger;

    public UpdateTripCommandHandler(
        ITripRepository tripRepository,
        IVehicleRepository vehicleRepository,
        IDriverRepository driverRepository,
        IValidator<UpdateTripCommand> validator,
        ILogger<UpdateTripCommandHandler> logger)
    {
        _tripRepository = tripRepository;
        _vehicleRepository = vehicleRepository;
        _driverRepository = driverRepository;
        _validator = validator;
        _logger = logger;
    }

    public Result<TripDto> Handle(Guid id, UpdateTripCommand command)
    {
        _logger.LogInformation(
            "Updating trip {TripId}",
            id);

        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                "; ",
                validationResult.Errors.Select(error => error.ErrorMessage));

            _logger.LogWarning(
                "Trip {TripId} validation failed: {ValidationErrors}",
                id,
                errors);

            return Result<TripDto>.Fail(errors);
        }

        var trip = _tripRepository.GetById(id);

        if (trip is null)
        {
            _logger.LogWarning(
                "Trip {TripId} not found",
                id);

            return Result<TripDto>.Fail("Trip not found.");
        }

        if (trip.StartDate <= DateTime.UtcNow)
        {
            _logger.LogWarning(
                "Trip {TripId} has already started",
                id);

            return Result<TripDto>.Fail(
                "Trip has already started and cannot be updated.");
        }

        var vehicle = _vehicleRepository.GetById(command.VehicleId);

        if (vehicle is null)
        {
            _logger.LogWarning(
                "Vehicle {VehicleId} not found while updating Trip {TripId}",
                command.VehicleId,
                id);

            return Result<TripDto>.Fail("Vehicle not found.");
        }

        var driver = _driverRepository.GetById(command.DriverId);

        if (driver is null)
        {
            _logger.LogWarning(
                "Driver {DriverId} not found while updating Trip {TripId}",
                command.DriverId,
                id);

            return Result<TripDto>.Fail("Driver not found.");
        }

        var conflictingVehicleTrip = _tripRepository
            .GetTripsByVehicle(command.VehicleId)
            .FirstOrDefault(t =>
                t.Id != id &&
                t.StartDate < command.EndDate &&
                command.StartDate < t.EndDate);

        if (conflictingVehicleTrip is not null)
        {
            _logger.LogWarning(
                "Vehicle {VehicleId} has a conflicting trip {ConflictingTripId}",
                command.VehicleId,
                conflictingVehicleTrip.Id);

            return Result<TripDto>.Fail(
                $"Vehicle is already assigned to another trip from {conflictingVehicleTrip.StartDate} to {conflictingVehicleTrip.EndDate}.");
        }

        var conflictingDriverTrip = _tripRepository
            .GetTripsByDriver(command.DriverId)
            .FirstOrDefault(t =>
                t.Id != id &&
                t.StartDate < command.EndDate &&
                command.StartDate < t.EndDate);

        if (conflictingDriverTrip is not null)
        {
            _logger.LogWarning(
                "Driver {DriverId} has a conflicting trip {ConflictingTripId}",
                command.DriverId,
                conflictingDriverTrip.Id);

            return Result<TripDto>.Fail(
                $"Driver is already assigned to another trip from {conflictingDriverTrip.StartDate} to {conflictingDriverTrip.EndDate}.");
        }

        trip.Update(
            command.VehicleId,
            command.DriverId,
            command.StartDate,
            command.EndDate);

        _tripRepository.Update(trip);
        _tripRepository.SaveChanges();

        _logger.LogInformation(
            "Trip {TripId} updated successfully",
            id);

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
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

    public Result<TripDto> Handle(UpdateTripCommand command)
    {
        _logger.LogInformation(
            "Updating trip {TripId}",
            command.Id);

        _validator.ValidateAndThrow(command);

        var trip = _tripRepository.GetById(command.Id);

        if (trip == null)
        {
            _logger.LogWarning(
                "Trip {TripId} not found.",
                command.Id);

            return Result<TripDto>.Fail("Trip not found.");
        }

        if (trip.StartDate <= DateTime.UtcNow)
        {
            _logger.LogWarning(
                "Trip {TripId} has already started.",
                command.Id);

            return Result<TripDto>.Fail(
                "Trip has already started and cannot be updated.");
        }

        var vehicle = _vehicleRepository.GetById(command.VehicleId);

        if (vehicle == null)
        {
            return Result<TripDto>.Fail("Vehicle not found.");
        }

        var driver = _driverRepository.GetById(command.DriverId);

        if (driver == null)
        {
            return Result<TripDto>.Fail("Driver not found.");
        }

        var conflictingVehicleTrip = _tripRepository
            .GetTripsByVehicle(command.VehicleId)
            .FirstOrDefault(t =>
                t.Id != command.Id &&
                t.StartDate < command.EndDate &&
                command.StartDate < t.EndDate);

        if (conflictingVehicleTrip != null)
        {
            return Result<TripDto>.Fail(
                $"Vehicle is already assigned to another trip from {conflictingVehicleTrip.StartDate} to {conflictingVehicleTrip.EndDate}.");
        }

        var conflictingDriverTrip = _tripRepository
            .GetTripsByDriver(command.DriverId)
            .FirstOrDefault(t =>
                t.Id != command.Id &&
                t.StartDate < command.EndDate &&
                command.StartDate < t.EndDate);

        if (conflictingDriverTrip != null)
        {
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
            "Trip {TripId} updated successfully.",
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
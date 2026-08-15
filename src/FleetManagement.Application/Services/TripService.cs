using FleetManagement.Application.DTOs;
using FleetManagement.Application.Shared;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Services
{
    public class TripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IValidator<CreateTripDto> _createValidator;
        private readonly IValidator<UpdateTripDto> _updateValidator;
        private readonly ILogger<TripService> _logger;

        public TripService(
            ITripRepository tripRepository,
            IVehicleRepository vehicleRepository,
            IDriverRepository driverRepository,
            IValidator<CreateTripDto> createValidator,
            IValidator<UpdateTripDto> updateValidator,
            ILogger<TripService> logger)
        {
            _tripRepository = tripRepository;
            _vehicleRepository = vehicleRepository;
            _driverRepository = driverRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public IEnumerable<TripDto> GetAll()
        {
            _logger.LogInformation("Fetching all trips");
            return _tripRepository.GetAll()
                .Select(t => new TripDto
                {
                    Id = t.Id,
                    VehicleId = t.VehicleId,
                    DriverId = t.DriverId,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate
                });
        }

        public TripDto? GetById(Guid id)
        {
            _logger.LogInformation("Fetching trip by Id {TripId}", id);
            var trip = _tripRepository.GetById(id);
            if (trip == null)
            {
                _logger.LogWarning("Trip {TripId} not found", id);
                return null;
            }

            return new TripDto
            {
                Id = trip.Id,
                VehicleId = trip.VehicleId,
                DriverId = trip.DriverId,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate
            };
        }

        public Result<TripDto> Add(CreateTripDto dto)
        {
            _logger.LogInformation("Adding new trip with Vehicle {VehicleId} and Driver {DriverId}", dto.VehicleId, dto.DriverId);
            _createValidator.ValidateAndThrow(dto);

            var validationResult = ValidateTrip(dto.VehicleId, dto.DriverId, dto.StartDate, dto.EndDate);
            if (!validationResult.IsSuccess)
            {
                _logger.LogWarning("Trip validation failed: {Error}", validationResult.Error);
                return Result<TripDto>.Fail(validationResult.Error);
            }

            var trip = new Trip(Guid.NewGuid(), dto.VehicleId, dto.DriverId, dto.StartDate, dto.EndDate);
            _tripRepository.Add(trip);
            _tripRepository.SaveChanges();

            _logger.LogInformation("Trip {TripId} created successfully", trip.Id);

            return Result<TripDto>.Ok(new TripDto
            {
                Id = trip.Id,
                VehicleId = trip.VehicleId,
                DriverId = trip.DriverId,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate
            });
        }

        public Result<TripDto> Update(Guid id, UpdateTripDto dto)
        {
            _logger.LogInformation("Updating trip {TripId}", id);
            _updateValidator.ValidateAndThrow(dto);

            var existing = _tripRepository.GetById(id);
            if (existing == null)
            {
                _logger.LogWarning("Trip {TripId} not found for update", id);
                return Result<TripDto>.Fail("Trip not found.");
            }

            if (existing.StartDate <= DateTime.UtcNow)
            {
                _logger.LogError("Trip {TripId} already started, update blocked", id);
                return Result<TripDto>.Fail("Trip has already started and cannot be updated.");
            }

            var vehicle = _vehicleRepository.GetById(dto.VehicleId);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle {VehicleId} not found for trip {TripId}", dto.VehicleId, id);
                return Result<TripDto>.Fail("Vehicle not found.");
            }

            var driver = _driverRepository.GetById(dto.DriverId);
            if (driver == null)
            {
                _logger.LogWarning("Driver {DriverId} not found for trip {TripId}", dto.DriverId, id);
                return Result<TripDto>.Fail("Driver not found.");
            }

            var conflictCheck = ValidateTrip(dto.VehicleId, dto.DriverId, dto.StartDate, dto.EndDate, id);
            if (!conflictCheck.IsSuccess)
            {
                _logger.LogError("Trip {TripId} update conflict: {Error}", id, conflictCheck.Error);
                return Result<TripDto>.Fail(conflictCheck.Error);
            }

            existing.Update(dto.VehicleId, dto.DriverId, dto.StartDate, dto.EndDate);
            _tripRepository.Update(existing);
            _tripRepository.SaveChanges();

            _logger.LogInformation("Trip {TripId} updated successfully", id);

            return Result<TripDto>.Ok(new TripDto
            {
                Id = existing.Id,
                VehicleId = existing.VehicleId,
                DriverId = existing.DriverId,
                StartDate = existing.StartDate,
                EndDate = existing.EndDate
            });
        }

        public Result<bool> Remove(Guid id)
        {
            _logger.LogInformation("Removing trip {TripId}", id);
            var trip = _tripRepository.GetById(id);
            if (trip == null)
            {
                _logger.LogWarning("Trip {TripId} not found for removal", id);
                return Result<bool>.Fail("Trip not found.");
            }

            _tripRepository.Remove(id);
            _tripRepository.SaveChanges();

            _logger.LogInformation("Trip {TripId} removed successfully", id);

            return Result<bool>.Ok(true);
        }

        private Result<bool> ValidateTrip(Guid vehicleId, Guid driverId, DateTime startDate, DateTime endDate, Guid? ignoreTripId = null)
        {
            _logger.LogInformation("Validating trip for Vehicle {VehicleId} and Driver {DriverId}", vehicleId, driverId);

            if (_vehicleRepository.GetById(vehicleId) == null)
                return Result<bool>.Fail("Vehicle not found.");

            if (_driverRepository.GetById(driverId) == null)
                return Result<bool>.Fail("Driver not found.");

            if (startDate <= DateTime.UtcNow)
                return Result<bool>.Fail("Cannot schedule a trip that has already started.");

            var conflictingTripVehicle = _tripRepository.GetTripsByVehicle(vehicleId)
                .FirstOrDefault(t => (ignoreTripId == null || t.Id != ignoreTripId) &&
                                     t.StartDate < endDate && startDate < t.EndDate);

            if (conflictingTripVehicle != null)
                return Result<bool>.Fail(
                    $"Vehicle is already assigned to another trip from {conflictingTripVehicle.StartDate} to {conflictingTripVehicle.EndDate}.");

            var conflictingTripDriver = _tripRepository.GetTripsByDriver(driverId)
                .FirstOrDefault(t => (ignoreTripId == null || t.Id != ignoreTripId) &&
                                     t.StartDate < endDate && startDate < t.EndDate);

            if (conflictingTripDriver != null)
                return Result<bool>.Fail(
                    $"Driver is already assigned to another trip from {conflictingTripDriver.StartDate} to {conflictingTripDriver.EndDate}.");

            return Result<bool>.Ok(true);
        }
    }
}

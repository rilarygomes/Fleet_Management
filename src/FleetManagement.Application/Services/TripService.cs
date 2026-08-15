using FleetManagement.Application.DTOs;
using FleetManagement.Application.Shared;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;

namespace FleetManagement.Application.Services
{
    public class TripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IValidator<CreateTripDto> _createValidator;
        private readonly IValidator<UpdateTripDto> _updateValidator;

        public TripService(
            ITripRepository tripRepository,
            IVehicleRepository vehicleRepository,
            IDriverRepository driverRepository,
            IValidator<CreateTripDto> createValidator,
            IValidator<UpdateTripDto> updateValidator)
        {
            _tripRepository = tripRepository;
            _vehicleRepository = vehicleRepository;
            _driverRepository = driverRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public IEnumerable<TripDto> GetAll()
        {
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
            var trip = _tripRepository.GetById(id);
            return trip == null ? null : new TripDto
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
            _createValidator.ValidateAndThrow(dto);

            var validationResult = ValidateTrip(dto.VehicleId, dto.DriverId, dto.StartDate, dto.EndDate);
            if (!validationResult.IsSuccess)
                return Result<TripDto>.Fail(validationResult.Error);

            var trip = new Trip(Guid.NewGuid(), dto.VehicleId, dto.DriverId, dto.StartDate, dto.EndDate);
            _tripRepository.Add(trip);
            _tripRepository.SaveChanges();

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
            // Validate incoming DTO
            _updateValidator.ValidateAndThrow(dto);

            // Fetch existing trip
            var existing = _tripRepository.GetById(id);
            if (existing == null)
                return Result<TripDto>.Fail("Trip not found.");

            // Business rule: if trip already started, block any update
            if (existing.StartDate <= DateTime.UtcNow)
                return Result<TripDto>.Fail("Trip has already started and cannot be updated.");

            // Ensure Vehicle exists
            var vehicle = _vehicleRepository.GetById(dto.VehicleId);
            if (vehicle == null)
                return Result<TripDto>.Fail("Vehicle not found.");

            // Ensure Driver exists
            var driver = _driverRepository.GetById(dto.DriverId);
            if (driver == null)
                return Result<TripDto>.Fail("Driver not found.");

            // Business rule: check conflicts (driver/vehicle overlap)
            var conflictCheck = ValidateTrip(dto.VehicleId, dto.DriverId, dto.StartDate, dto.EndDate, id);
            if (!conflictCheck.IsSuccess)
                return Result<TripDto>.Fail(conflictCheck.Error);

            // Apply update
            existing.Update(dto.VehicleId, dto.DriverId, dto.StartDate, dto.EndDate);
            _tripRepository.Update(existing);
            _tripRepository.SaveChanges();

            // Return updated DTO
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
            var trip = _tripRepository.GetById(id);
            if (trip == null)
                return Result<bool>.Fail("Trip not found.");

            _tripRepository.Remove(id);
            _tripRepository.SaveChanges();

            return Result<bool>.Ok(true);
        }

        private Result<bool> ValidateTrip(Guid vehicleId, Guid driverId, DateTime startDate, DateTime endDate, Guid? ignoreTripId = null)
        {
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

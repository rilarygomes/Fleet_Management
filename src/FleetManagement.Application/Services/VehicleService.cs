using FleetManagement.Application.DTOs;
using FleetManagement.Application.Shared;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Services
{
    public class VehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IValidator<CreateVehicleDto> _createValidator;
        private readonly IValidator<UpdateVehicleDto> _updateValidator;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(
            IVehicleRepository vehicleRepository,
            IValidator<CreateVehicleDto> createValidator,
            IValidator<UpdateVehicleDto> updateValidator,
            ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public IEnumerable<VehicleDto> GetAll()
        {
            _logger.LogInformation("Fetching all vehicles");
            return _vehicleRepository.GetAll()
                .Select(v => new VehicleDto
                {
                    Id = v.Id,
                    LicensePlate = v.LicensePlate,
                    Model = v.Model,
                    Year = v.Year
                });
        }

        public VehicleDto? GetById(Guid id)
        {
            _logger.LogInformation("Fetching vehicle by Id {VehicleId}", id);
            var vehicle = _vehicleRepository.GetById(id);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle {VehicleId} not found", id);
                return null;
            }

            return new VehicleDto
            {
                Id = vehicle.Id,
                LicensePlate = vehicle.LicensePlate,
                Model = vehicle.Model,
                Year = vehicle.Year
            };
        }

        public Result<VehicleDto> Add(CreateVehicleDto dto)
        {
            _logger.LogInformation("Adding new vehicle with LicensePlate {LicensePlate}", dto.LicensePlate);
            _createValidator.ValidateAndThrow(dto);

            var existing = _vehicleRepository.GetByLicensePlate(dto.LicensePlate);
            if (existing != null)
            {
                _logger.LogWarning("Vehicle with LicensePlate {LicensePlate} already exists", dto.LicensePlate);
                return Result<VehicleDto>.Fail("A vehicle with this license plate already exists.");
            }

            var vehicle = new Vehicle(Guid.NewGuid(), dto.LicensePlate, dto.Model, dto.Year);
            _vehicleRepository.Add(vehicle);
            _vehicleRepository.SaveChanges();

            _logger.LogInformation("Vehicle {VehicleId} created successfully", vehicle.Id);

            return Result<VehicleDto>.Ok(new VehicleDto
            {
                Id = vehicle.Id,
                LicensePlate = vehicle.LicensePlate,
                Model = vehicle.Model,
                Year = vehicle.Year
            });
        }

        public Result<VehicleDto> Update(Guid id, UpdateVehicleDto dto)
        {
            _logger.LogInformation("Updating vehicle {VehicleId}", id);
            _updateValidator.ValidateAndThrow(dto);

            var existing = _vehicleRepository.GetById(id);
            if (existing == null)
            {
                _logger.LogWarning("Vehicle {VehicleId} not found for update", id);
                return Result<VehicleDto>.Fail("Vehicle not found.");
            }

            existing.Update(dto.LicensePlate, dto.Model, dto.Year);
            _vehicleRepository.Update(existing);
            _vehicleRepository.SaveChanges();

            _logger.LogInformation("Vehicle {VehicleId} updated successfully", id);

            return Result<VehicleDto>.Ok(new VehicleDto
            {
                Id = existing.Id,
                LicensePlate = existing.LicensePlate,
                Model = existing.Model,
                Year = existing.Year
            });
        }

        public Result<bool> Remove(Guid id)
        {
            _logger.LogInformation("Removing vehicle {VehicleId}", id);
            var vehicle = _vehicleRepository.GetById(id);
            if (vehicle == null)
            {
                _logger.LogWarning("Vehicle {VehicleId} not found for removal", id);
                return Result<bool>.Fail("Vehicle not found.");
            }

            if (_vehicleRepository.HasTrips(id))
            {
                _logger.LogError("Cannot delete vehicle {VehicleId} because trips are associated", id);
                return Result<bool>.Fail("Cannot delete vehicle because there are trips associated with this vehicle.");
            }

            _vehicleRepository.Remove(id);
            _vehicleRepository.SaveChanges();

            _logger.LogInformation("Vehicle {VehicleId} removed successfully", id);

            return Result<bool>.Ok(true);
        }
    }
}

using FleetManagement.Application.DTOs;
using FleetManagement.Application.Shared;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;

namespace FleetManagement.Application.Services
{
    public class VehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IValidator<CreateVehicleDto> _createValidator;
        private readonly IValidator<UpdateVehicleDto> _updateValidator;

        public VehicleService(
            IVehicleRepository vehicleRepository,
            IValidator<CreateVehicleDto> createValidator,
            IValidator<UpdateVehicleDto> updateValidator)
        {
            _vehicleRepository = vehicleRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public IEnumerable<VehicleDto> GetAll()
        {
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
            var vehicle = _vehicleRepository.GetById(id);
            return vehicle == null ? null : new VehicleDto
            {
                Id = vehicle.Id,
                LicensePlate = vehicle.LicensePlate,
                Model = vehicle.Model,
                Year = vehicle.Year
            };
        }

        public Result<VehicleDto> Add(CreateVehicleDto dto)
        {
            _createValidator.ValidateAndThrow(dto);

            var existing = _vehicleRepository.GetByLicensePlate(dto.LicensePlate);
            if (existing != null)
                return Result<VehicleDto>.Fail("A vehicle with this license plate already exists.");

            var vehicle = new Vehicle(Guid.NewGuid(), dto.LicensePlate, dto.Model, dto.Year);
            _vehicleRepository.Add(vehicle);
            _vehicleRepository.SaveChanges();

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
            _updateValidator.ValidateAndThrow(dto);

            var existing = _vehicleRepository.GetById(id);
            if (existing == null)
                return Result<VehicleDto>.Fail("Vehicle not found.");

            existing.Update(dto.LicensePlate, dto.Model, dto.Year);
            _vehicleRepository.Update(existing);
            _vehicleRepository.SaveChanges();

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
            var vehicle = _vehicleRepository.GetById(id);
            if (vehicle == null)
                return Result<bool>.Fail("Vehicle not found.");

            if (_vehicleRepository.HasTrips(id))
                return Result<bool>.Fail("Cannot delete vehicle because there are trips associated with this vehicle.");

            _vehicleRepository.Remove(id);
            _vehicleRepository.SaveChanges();

            return Result<bool>.Ok(true);
        }
    }
}

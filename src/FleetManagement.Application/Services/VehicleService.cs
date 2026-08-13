using FleetManagement.Application.DTOs;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;

namespace FleetManagement.Application.Services
{
    public class VehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IValidator<VehicleDto> _validator;

        public VehicleService(IVehicleRepository vehicleRepository, IValidator<VehicleDto> validator)
        {
            _vehicleRepository = vehicleRepository;
            _validator = validator;
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

        public void Add(VehicleDto dto)
        {
            _validator.ValidateAndThrow(dto);

            var vehicle = new Vehicle(
                Guid.NewGuid(),
                dto.LicensePlate,    
                dto.Model,           
                dto.Year             
            );

            _vehicleRepository.Add(vehicle);
        }

        public void Update(VehicleDto dto)
        {
            _validator.ValidateAndThrow(dto);

            var vehicle = new Vehicle(
                dto.Id,
                dto.LicensePlate,
                dto.Model,
                dto.Year
            );

            _vehicleRepository.Update(vehicle);
        }

        public void Remove(Guid id) => _vehicleRepository.Remove(id);
    }
}

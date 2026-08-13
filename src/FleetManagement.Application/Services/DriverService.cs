using FleetManagement.Application.DTOs;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;

namespace FleetManagement.Application.Services
{
    public class DriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IValidator<DriverDto> _validator;

        public DriverService(IDriverRepository driverRepository, IValidator<DriverDto> validator)
        {
            _driverRepository = driverRepository;
            _validator = validator;
        }

        public IEnumerable<DriverDto> GetAll()
        {
            return _driverRepository.GetAll()
                .Select(d => new DriverDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    LicenseNumber = d.LicenseNumber
                });
        }

        public DriverDto? GetById(Guid id)
        {
            var driver = _driverRepository.GetById(id);
            return driver == null ? null : new DriverDto
            {
                Id = driver.Id,
                Name = driver.Name,
                LicenseNumber = driver.LicenseNumber
            };
        }

        public void Add(DriverDto dto)
        {
            _validator.ValidateAndThrow(dto);

            var driver = new Driver(
                Guid.NewGuid(),          
                dto.Name,
                dto.LicenseNumber,
                dto.LicenseExpirationDate
            );

            _driverRepository.Add(driver);
        }

        public void Update(DriverDto dto)
        {
            _validator.ValidateAndThrow(dto);

            var driver = new Driver(
                dto.Id,                  
                dto.Name,
                dto.LicenseNumber,
                dto.LicenseExpirationDate
            );

            _driverRepository.Update(driver);
        }

        public void Remove(Guid id) => _driverRepository.Remove(id);
    }
}

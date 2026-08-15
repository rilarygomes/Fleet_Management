using FleetManagement.Application.DTOs;
using FleetManagement.Application.Shared;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;

namespace FleetManagement.Application.Services
{
    public class DriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IValidator<CreateDriverDto> _createValidator;
        private readonly IValidator<UpdateDriverDto> _updateValidator;

        public DriverService(
            IDriverRepository driverRepository,
            IValidator<CreateDriverDto> createValidator,
            IValidator<UpdateDriverDto> updateValidator)
        {
            _driverRepository = driverRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public IEnumerable<DriverDto> GetAll()
        {
            return _driverRepository.GetAll()
                .Select(d => new DriverDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    LicenseNumber = d.LicenseNumber,
                    LicenseExpirationDate = d.LicenseExpirationDate
                });
        }

        public DriverDto? GetById(Guid id)
        {
            var driver = _driverRepository.GetById(id);
            return driver == null ? null : new DriverDto
            {
                Id = driver.Id,
                Name = driver.Name,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpirationDate = driver.LicenseExpirationDate
            };
        }

        public Result<DriverDto> Add(CreateDriverDto dto)
        {
            _createValidator.ValidateAndThrow(dto);

            var existing = _driverRepository.GetByLicenseNumber(dto.LicenseNumber);
            if (existing != null)
                return Result<DriverDto>.Fail("A driver with this license number already exists.");

            var driver = new Driver(Guid.NewGuid(), dto.Name, dto.LicenseNumber, dto.LicenseExpirationDate);
            _driverRepository.Add(driver);
            _driverRepository.SaveChanges();

            return Result<DriverDto>.Ok(new DriverDto
            {
                Id = driver.Id,
                Name = driver.Name,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpirationDate = driver.LicenseExpirationDate
            });
        }

        public Result<DriverDto> Update(Guid id, UpdateDriverDto dto)
        {
            _updateValidator.ValidateAndThrow(dto);

            var existing = _driverRepository.GetById(id);
            if (existing == null)
                return Result<DriverDto>.Fail("Driver not found.");

            existing.Update(dto.Name, dto.LicenseNumber, dto.LicenseExpirationDate);
            _driverRepository.Update(existing);
            _driverRepository.SaveChanges();

            return Result<DriverDto>.Ok(new DriverDto
            {
                Id = existing.Id,
                Name = existing.Name,
                LicenseNumber = existing.LicenseNumber,
                LicenseExpirationDate = existing.LicenseExpirationDate
            });
        }

        public Result<bool> Remove(Guid id)
        {
            var driver = _driverRepository.GetById(id);
            if (driver == null)
                return Result<bool>.Fail("Driver not found.");

            if (_driverRepository.HasTrips(id))
                return Result<bool>.Fail("Cannot delete driver because there are trips associated with this driver.");

            _driverRepository.Remove(id);
            _driverRepository.SaveChanges();

            return Result<bool>.Ok(true);
        }
    }
}

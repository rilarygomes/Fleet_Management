using FleetManagement.Application.DTOs;
using FleetManagement.Application.Shared;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Services
{
    public class DriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IValidator<CreateDriverDto> _createValidator;
        private readonly IValidator<UpdateDriverDto> _updateValidator;
        private readonly ILogger<DriverService> _logger;

        public DriverService(
            IDriverRepository driverRepository,
            IValidator<CreateDriverDto> createValidator,
            IValidator<UpdateDriverDto> updateValidator,
            ILogger<DriverService> logger)
        {
            _driverRepository = driverRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public IEnumerable<DriverDto> GetAll()
        {
            _logger.LogInformation("Fetching all drivers");
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
            _logger.LogInformation("Fetching driver by Id {DriverId}", id);
            var driver = _driverRepository.GetById(id);
            if (driver == null)
            {
                _logger.LogWarning("Driver {DriverId} not found", id);
                return null;
            }

            return new DriverDto
            {
                Id = driver.Id,
                Name = driver.Name,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpirationDate = driver.LicenseExpirationDate
            };
        }

        public Result<DriverDto> Add(CreateDriverDto dto)
        {
            _logger.LogInformation("Adding new driver with LicenseNumber {LicenseNumber}", dto.LicenseNumber);
            _createValidator.ValidateAndThrow(dto);

            var existing = _driverRepository.GetByLicenseNumber(dto.LicenseNumber);
            if (existing != null)
            {
                _logger.LogWarning("Driver with LicenseNumber {LicenseNumber} already exists", dto.LicenseNumber);
                return Result<DriverDto>.Fail("A driver with this license number already exists.");
            }

            var driver = new Driver(Guid.NewGuid(), dto.Name, dto.LicenseNumber, dto.LicenseExpirationDate);
            _driverRepository.Add(driver);
            _driverRepository.SaveChanges();

            _logger.LogInformation("Driver {DriverId} created successfully", driver.Id);

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
            _logger.LogInformation("Updating driver {DriverId}", id);
            _updateValidator.ValidateAndThrow(dto);

            var existing = _driverRepository.GetById(id);
            if (existing == null)
            {
                _logger.LogWarning("Driver {DriverId} not found for update", id);
                return Result<DriverDto>.Fail("Driver not found.");
            }

            existing.Update(dto.Name, dto.LicenseNumber, dto.LicenseExpirationDate);
            _driverRepository.Update(existing);
            _driverRepository.SaveChanges();

            _logger.LogInformation("Driver {DriverId} updated successfully", id);

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
            _logger.LogInformation("Removing driver {DriverId}", id);
            var driver = _driverRepository.GetById(id);
            if (driver == null)
            {
                _logger.LogWarning("Driver {DriverId} not found for removal", id);
                return Result<bool>.Fail("Driver not found.");
            }

            if (_driverRepository.HasTrips(id))
            {
                _logger.LogError("Cannot delete driver {DriverId} because trips are associated", id);
                return Result<bool>.Fail("Cannot delete driver because there are trips associated with this driver.");
            }

            _driverRepository.Remove(id);
            _driverRepository.SaveChanges();

            _logger.LogInformation("Driver {DriverId} removed successfully", id);

            return Result<bool>.Ok(true);
        }
    }
}

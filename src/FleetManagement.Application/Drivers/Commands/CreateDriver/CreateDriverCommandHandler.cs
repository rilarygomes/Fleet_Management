using FleetManagement.Application.Drivers.DTOs;
using FleetManagement.Application.Shared;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Drivers.Commands.CreateDriver;

public class CreateDriverCommandHandler
{
    private readonly IDriverRepository _driverRepository;
    private readonly IValidator<CreateDriverCommand> _validator;
    private readonly ILogger<CreateDriverCommandHandler> _logger;

    public CreateDriverCommandHandler(
        IDriverRepository driverRepository,
        IValidator<CreateDriverCommand> validator,
        ILogger<CreateDriverCommandHandler> logger)
    {
        _driverRepository = driverRepository;
        _validator = validator;
        _logger = logger;
    }

    public Result<DriverDto> Handle(CreateDriverCommand command)
    {
        _logger.LogInformation(
            "Creating driver with LicenseNumber {LicenseNumber}",
            command.LicenseNumber);

        _validator.ValidateAndThrow(command);

        var existing = _driverRepository.GetByLicenseNumber(command.LicenseNumber);

        if (existing is not null)
        {
            _logger.LogWarning(
                "Driver with LicenseNumber {LicenseNumber} already exists",
                command.LicenseNumber);

            return Result<DriverDto>.Fail(
                "A driver with this license number already exists.");
        }

        var driver = new Domain.Entities.Driver(
            Guid.NewGuid(),
            command.Name,
            command.LicenseNumber,
            command.LicenseExpirationDate);

        _driverRepository.Add(driver);
        _driverRepository.SaveChanges();

        _logger.LogInformation(
            "Driver {DriverId} created successfully",
            driver.Id);

        return Result<DriverDto>.Ok(new DriverDto
        {
            Id = driver.Id,
            Name = driver.Name,
            LicenseNumber = driver.LicenseNumber,
            LicenseExpirationDate = driver.LicenseExpirationDate
        });
    }
}
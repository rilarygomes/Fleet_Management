using FleetManagement.Application.Drivers.DTOs;
using FleetManagement.Application.Shared;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Drivers.Commands.UpdateDriver;

public class UpdateDriverCommandHandler
{
    private readonly IDriverRepository _driverRepository;
    private readonly IValidator<UpdateDriverCommand> _validator;
    private readonly ILogger<UpdateDriverCommandHandler> _logger;

    public UpdateDriverCommandHandler(
        IDriverRepository driverRepository,
        IValidator<UpdateDriverCommand> validator,
        ILogger<UpdateDriverCommandHandler> logger)
    {
        _driverRepository = driverRepository;
        _validator = validator;
        _logger = logger;
    }

    public Result<DriverDto> Handle(Guid id, UpdateDriverCommand command)
    {
        _logger.LogInformation(
            "Updating driver {DriverId}",
            id);

        var validationResult = _validator.Validate(command);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(
                "; ",
                validationResult.Errors.Select(error => error.ErrorMessage));

            _logger.LogWarning(
                "Driver {DriverId} validation failed: {ValidationErrors}",
                id,
                errors);

            return Result<DriverDto>.Fail(errors);
        }

        var driver = _driverRepository.GetById(id);

        if (driver is null)
        {
            _logger.LogWarning(
                "Driver {DriverId} not found",
                id);

            return Result<DriverDto>.Fail("Driver not found.");
        }

        var duplicate = _driverRepository
            .GetByLicenseNumber(command.LicenseNumber);

        if (duplicate is not null && duplicate.Id != id)
        {
            _logger.LogWarning(
                "License number {LicenseNumber} already belongs to another driver",
                command.LicenseNumber);

            return Result<DriverDto>.Fail(
                "Another driver already uses this license number.");
        }

        driver.Update(
            command.Name,
            command.LicenseNumber,
            command.LicenseExpirationDate);

        _driverRepository.Update(driver);
        _driverRepository.SaveChanges();

        _logger.LogInformation(
            "Driver {DriverId} updated successfully",
            id);

        return Result<DriverDto>.Ok(new DriverDto
        {
            Id = driver.Id,
            Name = driver.Name,
            LicenseNumber = driver.LicenseNumber,
            LicenseExpirationDate = driver.LicenseExpirationDate
        });
    }
}
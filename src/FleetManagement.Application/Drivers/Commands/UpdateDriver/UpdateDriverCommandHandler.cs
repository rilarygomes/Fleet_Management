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

    public Result<DriverDto> Handle(UpdateDriverCommand command)
    {
        _logger.LogInformation(
            "Updating driver {DriverId}",
            command.Id);

        _validator.ValidateAndThrow(command);

        var driver = _driverRepository.GetById(command.Id);

        if (driver == null)
        {
            _logger.LogWarning(
                "Driver {DriverId} not found.",
                command.Id);

            return Result<DriverDto>.Fail("Driver not found.");
        }

        var duplicate = _driverRepository.GetByLicenseNumber(command.LicenseNumber);

        if (duplicate != null && duplicate.Id != command.Id)
        {
            _logger.LogWarning(
                "License number {LicenseNumber} already belongs to another driver.",
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
            "Driver {DriverId} updated successfully.",
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
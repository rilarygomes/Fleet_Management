using FleetManagement.Application.Drivers.DTOs;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Drivers.GetDriver;

public class GetDriverQueryHandler
{
    private readonly IDriverRepository _driverRepository;
    private readonly ILogger<GetDriverQueryHandler> _logger;

    public GetDriverQueryHandler(
        IDriverRepository driverRepository,
        ILogger<GetDriverQueryHandler> logger)
    {
        _driverRepository = driverRepository;
        _logger = logger;
    }

    public DriverDto? Handle(GetDriverQuery query)
    {
        _logger.LogInformation(
            "Fetching driver with Id {DriverId}",
            query.Id);

        var driver = _driverRepository.GetById(query.Id);

        if (driver == null)
        {
            _logger.LogWarning(
                "Driver {DriverId} not found",
                query.Id);

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
}
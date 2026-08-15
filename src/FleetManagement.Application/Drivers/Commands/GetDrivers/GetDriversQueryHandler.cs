using FleetManagement.Application.Drivers.DTOs;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Drivers.GetDrivers;

public class GetDriversQueryHandler
{
    private readonly IDriverRepository _driverRepository;
    private readonly ILogger<GetDriversQueryHandler> _logger;

    public GetDriversQueryHandler(
        IDriverRepository driverRepository,
        ILogger<GetDriversQueryHandler> logger)
    {
        _driverRepository = driverRepository;
        _logger = logger;
    }

    public IEnumerable<DriverDto> Handle(GetDriversQuery query)
    {
        _logger.LogInformation(
            "Fetching drivers with filter Name={Name}",
            query.Name);

        var drivers = _driverRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            drivers = drivers.Where(d =>
                d.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));
        }

        return drivers.Select(driver => new DriverDto
        {
            Id = driver.Id,
            Name = driver.Name,
            LicenseNumber = driver.LicenseNumber,
            LicenseExpirationDate = driver.LicenseExpirationDate
        });
    }
}
using FleetManagement.Application.Shared;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Drivers.Commands.DeleteDriver;

public class DeleteDriverCommandHandler
{
    private readonly IDriverRepository _driverRepository;
    private readonly ILogger<DeleteDriverCommandHandler> _logger;

    public DeleteDriverCommandHandler(
        IDriverRepository driverRepository,
        ILogger<DeleteDriverCommandHandler> logger)
    {
        _driverRepository = driverRepository;
        _logger = logger;
    }

    public Result<bool> Handle(DeleteDriverCommand command)
    {
        _logger.LogInformation(
            "Removing driver {DriverId}",
            command.Id);

        var driver = _driverRepository.GetById(command.Id);

        if (driver == null)
        {
            _logger.LogWarning(
                "Driver {DriverId} not found for removal",
                command.Id);

            return Result<bool>.Fail("Driver not found.");
        }

        if (_driverRepository.HasTrips(command.Id))
        {
            _logger.LogWarning(
                "Cannot delete driver {DriverId} because trips are associated.",
                command.Id);

            return Result<bool>.Fail(
                "Cannot delete driver because there are trips associated with this driver.");
        }

        _driverRepository.Remove(command.Id);
        _driverRepository.SaveChanges();

        _logger.LogInformation(
            "Driver {DriverId} removed successfully",
            command.Id);

        return Result<bool>.Ok(true);
    }
}
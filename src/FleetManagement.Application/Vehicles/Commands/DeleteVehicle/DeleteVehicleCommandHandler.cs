using FleetManagement.Application.Shared;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Vehicles.Commands.DeleteVehicle;

public class DeleteVehicleCommandHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<DeleteVehicleCommandHandler> _logger;

    public DeleteVehicleCommandHandler(
        IVehicleRepository vehicleRepository,
        ILogger<DeleteVehicleCommandHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public Result<bool> Handle(DeleteVehicleCommand command)
    {
        _logger.LogInformation(
            "Removing vehicle {VehicleId}",
            command.Id);

        var vehicle = _vehicleRepository.GetById(command.Id);

        if (vehicle == null)
        {
            _logger.LogWarning(
                "Vehicle {VehicleId} not found for removal",
                command.Id);

            return Result<bool>.Fail("Vehicle not found.");
        }

        if (_vehicleRepository.HasTrips(command.Id))
        {
            _logger.LogWarning(
                "Cannot delete vehicle {VehicleId} because it has associated trips.",
                command.Id);

            return Result<bool>.Fail(
                "Cannot delete vehicle because there are trips associated with this vehicle.");
        }

        _vehicleRepository.Remove(command.Id);
        _vehicleRepository.SaveChanges();

        _logger.LogInformation(
            "Vehicle {VehicleId} removed successfully.",
            command.Id);

        return Result<bool>.Ok(true);
    }
}
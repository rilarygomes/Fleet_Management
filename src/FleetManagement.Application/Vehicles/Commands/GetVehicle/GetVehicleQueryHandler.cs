using FleetManagement.Application.Vehicles.DTOs;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Vehicles.GetVehicle;

public class GetVehicleQueryHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<GetVehicleQueryHandler> _logger;

    public GetVehicleQueryHandler(
        IVehicleRepository vehicleRepository,
        ILogger<GetVehicleQueryHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public VehicleDto? Handle(GetVehicleQuery query)
    {
        _logger.LogInformation(
            "Fetching vehicle {VehicleId}",
            query.Id);

        var vehicle = _vehicleRepository.GetById(query.Id);

        if (vehicle == null)
        {
            _logger.LogWarning(
                "Vehicle {VehicleId} not found.",
                query.Id);

            return null;
        }

        return new VehicleDto
        {
            Id = vehicle.Id,
            LicensePlate = vehicle.LicensePlate,
            Model = vehicle.Model,
            Year = vehicle.Year
        };
    }
}
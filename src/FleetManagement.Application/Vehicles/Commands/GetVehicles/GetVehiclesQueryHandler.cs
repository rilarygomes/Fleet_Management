using FleetManagement.Application.Vehicles.DTOs;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Vehicles.GetVehicles;

public class GetVehiclesQueryHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILogger<GetVehiclesQueryHandler> _logger;

    public GetVehiclesQueryHandler(
        IVehicleRepository vehicleRepository,
        ILogger<GetVehiclesQueryHandler> logger)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
    }

    public IEnumerable<VehicleDto> Handle(GetVehiclesQuery query)
    {
        _logger.LogInformation(
            "Fetching vehicles with filter LicensePlate={LicensePlate}",
            query.LicensePlate);

        var vehicles = _vehicleRepository.GetAll();

        if (!string.IsNullOrWhiteSpace(query.LicensePlate))
        {
            vehicles = vehicles.Where(v =>
                v.LicensePlate.Contains(
                    query.LicensePlate,
                    StringComparison.OrdinalIgnoreCase));
        }

        return vehicles.Select(v => new VehicleDto
        {
            Id = v.Id,
            LicensePlate = v.LicensePlate,
            Model = v.Model,
            Year = v.Year
        });
    }
}
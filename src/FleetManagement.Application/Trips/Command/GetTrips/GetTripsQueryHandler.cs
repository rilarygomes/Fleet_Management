using FleetManagement.Application.Trips.DTOs;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Trips.GetTrips;

public class GetTripsQueryHandler
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<GetTripsQueryHandler> _logger;

    public GetTripsQueryHandler(
        ITripRepository tripRepository,
        ILogger<GetTripsQueryHandler> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public IEnumerable<TripDto> Handle(GetTripsQuery query)
    {
        _logger.LogInformation(
            "Fetching trips with filters StartDate={StartDate}, EndDate={EndDate}, DriverId={DriverId}, VehicleId={VehicleId}",
            query.StartDate,
            query.EndDate,
            query.DriverId,
            query.VehicleId);

        var trips = _tripRepository.GetAll();

        if (query.StartDate.HasValue)
        {
            trips = trips.Where(t =>
                t.StartDate >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            trips = trips.Where(t =>
                t.EndDate <= query.EndDate.Value);
        }

        if (query.DriverId.HasValue)
        {
            trips = trips.Where(t =>
                t.DriverId == query.DriverId.Value);
        }

        if (query.VehicleId.HasValue)
        {
            trips = trips.Where(t =>
                t.VehicleId == query.VehicleId.Value);
        }

        return trips.Select(t => new TripDto
        {
            Id = t.Id,
            VehicleId = t.VehicleId,
            DriverId = t.DriverId,
            StartDate = t.StartDate,
            EndDate = t.EndDate
        });
    }
}
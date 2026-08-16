using FleetManagement.Application.Trips.DTOs;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Trips.GetTrip;

public class GetTripQueryHandler
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<GetTripQueryHandler> _logger;

    public GetTripQueryHandler(
        ITripRepository tripRepository,
        ILogger<GetTripQueryHandler> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public TripDto? Handle(GetTripQuery query)
    {
        _logger.LogInformation(
            "Fetching trip {TripId}",
            query.Id);

        var trip = _tripRepository.GetById(query.Id);

        if (trip == null)
        {
            _logger.LogWarning(
                "Trip {TripId} not found",
                query.Id);

            return null;
        }

        return new TripDto
        {
            Id = trip.Id,
            VehicleId = trip.VehicleId,
            DriverId = trip.DriverId,
            StartDate = trip.StartDate,
            EndDate = trip.EndDate
        };
    }
}
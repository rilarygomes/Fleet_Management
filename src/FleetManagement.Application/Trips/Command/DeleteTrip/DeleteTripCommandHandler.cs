using FleetManagement.Application.Shared;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FleetManagement.Application.Trips.Commands.DeleteTrip;

public class DeleteTripCommandHandler
{
    private readonly ITripRepository _tripRepository;
    private readonly ILogger<DeleteTripCommandHandler> _logger;

    public DeleteTripCommandHandler(
        ITripRepository tripRepository,
        ILogger<DeleteTripCommandHandler> logger)
    {
        _tripRepository = tripRepository;
        _logger = logger;
    }

    public Result<bool> Handle(DeleteTripCommand command)
    {
        _logger.LogInformation(
            "Removing trip {TripId}",
            command.Id);

        var trip = _tripRepository.GetById(command.Id);

        if (trip == null)
        {
            _logger.LogWarning(
                "Trip {TripId} not found for removal",
                command.Id);

            return Result<bool>.Fail("Trip not found.");
        }

        _tripRepository.Remove(command.Id);
        _tripRepository.SaveChanges();

        _logger.LogInformation(
            "Trip {TripId} removed successfully",
            command.Id);

        return Result<bool>.Ok(true);
    }
}
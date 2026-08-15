using FleetManagement.Application.Trips.GetTrip;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FleetManagement.UnitTests.Trips;

public class GetTripQueryHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<ILogger<GetTripQueryHandler>> _loggerMock;
    private readonly GetTripQueryHandler _handler;

    public GetTripQueryHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _loggerMock = new Mock<ILogger<GetTripQueryHandler>>();

        _handler = new GetTripQueryHandler(
            _tripRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Return_Null_When_Trip_Not_Found()
    {
        var query = new GetTripQuery
        {
            Id = Guid.NewGuid()
        };

        _tripRepositoryMock
            .Setup(r => r.GetById(query.Id))
            .Returns((Trip?)null);

        var result = _handler.Handle(query);

        Assert.Null(result);
    }

    [Fact]
    public void Handle_Should_Return_Trip_When_Found()
    {
        var trip = new Trip(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2));

        _tripRepositoryMock
            .Setup(r => r.GetById(trip.Id))
            .Returns(trip);

        var result = _handler.Handle(new GetTripQuery
        {
            Id = trip.Id
        });

        Assert.NotNull(result);
        Assert.Equal(trip.Id, result!.Id);
        Assert.Equal(trip.VehicleId, result.VehicleId);
        Assert.Equal(trip.DriverId, result.DriverId);
        Assert.Equal(trip.StartDate, result.StartDate);
        Assert.Equal(trip.EndDate, result.EndDate);
    }
}
using FleetManagement.Application.Trips.GetTrips;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FleetManagement.UnitTests.Trips;

public class GetTripsQueryHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<ILogger<GetTripsQueryHandler>> _loggerMock;
    private readonly GetTripsQueryHandler _handler;

    public GetTripsQueryHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _loggerMock = new Mock<ILogger<GetTripsQueryHandler>>();

        _handler = new GetTripsQueryHandler(
            _tripRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Return_All_Trips_When_No_Filters_Are_Provided()
    {
        var trips = CreateTrips();

        _tripRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(trips);

        var result = _handler
            .Handle(new GetTripsQuery())
            .ToList();

        Assert.Equal(2, result.Count);

        _tripRepositoryMock.Verify(
            r => r.GetAll(),
            Times.Once);
    }

    [Fact]
    public void Handle_Should_Filter_By_DriverId()
    {
        var trips = CreateTrips();

        _tripRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(trips);

        var result = _handler.Handle(new GetTripsQuery
        {
            DriverId = trips[0].DriverId
        }).ToList();

        Assert.Single(result);
        Assert.Equal(trips[0].DriverId, result[0].DriverId);
    }

    [Fact]
    public void Handle_Should_Filter_By_VehicleId()
    {
        var trips = CreateTrips();

        _tripRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(trips);

        var result = _handler.Handle(new GetTripsQuery
        {
            VehicleId = trips[1].VehicleId
        }).ToList();

        Assert.Single(result);
        Assert.Equal(trips[1].VehicleId, result[0].VehicleId);
    }

    [Fact]
    public void Handle_Should_Filter_By_Date_Range()
    {
        var trips = CreateTrips();

        _tripRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(trips);

        var result = _handler.Handle(new GetTripsQuery
        {
            StartDate = DateTime.UtcNow.AddDays(3),
            EndDate = DateTime.UtcNow.AddDays(5)
        }).ToList();

        Assert.Single(result);
        Assert.Equal(trips[1].Id, result[0].Id);
    }

    private static List<Trip> CreateTrips()
    {
        return new List<Trip>
        {
            new(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(2)),

            new(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow.AddDays(3),
                DateTime.UtcNow.AddDays(4))
        };
    }
}
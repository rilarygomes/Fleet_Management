using FleetManagement.Application.Trips.Commands.DeleteTrip;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FleetManagement.UnitTests.Trips;

public class DeleteTripCommandHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<ILogger<DeleteTripCommandHandler>> _loggerMock;
    private readonly DeleteTripCommandHandler _handler;

    public DeleteTripCommandHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _loggerMock = new Mock<ILogger<DeleteTripCommandHandler>>();

        _handler = new DeleteTripCommandHandler(
            _tripRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Fail_When_Trip_Not_Found()
    {
        var command = new DeleteTripCommand
        {
            Id = Guid.NewGuid()
        };

        _tripRepositoryMock
            .Setup(r => r.GetById(command.Id))
            .Returns((Trip?)null);

        var result = _handler.Handle(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Trip not found.", result.Error);
    }

    [Fact]
    public void Handle_Should_Succeed_When_Trip_Exists()
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

        var result = _handler.Handle(new DeleteTripCommand
        {
            Id = trip.Id
        });

        Assert.True(result.IsSuccess);

        _tripRepositoryMock.Verify(
            r => r.Remove(trip.Id),
            Times.Once);

        _tripRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }
}
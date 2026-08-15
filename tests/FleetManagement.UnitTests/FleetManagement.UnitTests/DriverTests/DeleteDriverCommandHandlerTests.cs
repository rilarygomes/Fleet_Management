using FleetManagement.Application.Drivers.Commands.DeleteDriver;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class DeleteDriverCommandHandlerTests
{
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<ILogger<DeleteDriverCommandHandler>> _loggerMock;
    private readonly DeleteDriverCommandHandler _handler;

    public DeleteDriverCommandHandlerTests()
    {
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _loggerMock = new Mock<ILogger<DeleteDriverCommandHandler>>();

        _handler = new DeleteDriverCommandHandler(
            _driverRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Fail_When_Driver_Not_Found()
    {
        var command = new DeleteDriverCommand
        {
            Id = Guid.NewGuid()
        };

        _driverRepositoryMock
            .Setup(r => r.GetById(command.Id))
            .Returns((Driver?)null);

        var result = _handler.Handle(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Driver not found.", result.Error);
    }

    [Fact]
    public void Handle_Should_Fail_When_Driver_Has_Trips()
    {
        var existing = new Driver(
            Guid.NewGuid(),
            "Carlos",
            "12345678901",
            DateTime.UtcNow.AddYears(1));

        _driverRepositoryMock
            .Setup(r => r.GetById(existing.Id))
            .Returns(existing);

        _driverRepositoryMock
            .Setup(r => r.HasTrips(existing.Id))
            .Returns(true);

        var result = _handler.Handle(new DeleteDriverCommand
        {
            Id = existing.Id
        });

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Cannot delete driver because there are trips associated with this driver.",
            result.Error);
    }

    [Fact]
    public void Handle_Should_Succeed_When_Driver_Has_No_Trips()
    {
        var existing = new Driver(
            Guid.NewGuid(),
            "Carlos",
            "12345678901",
            DateTime.UtcNow.AddYears(1));

        _driverRepositoryMock
            .Setup(r => r.GetById(existing.Id))
            .Returns(existing);

        _driverRepositoryMock
            .Setup(r => r.HasTrips(existing.Id))
            .Returns(false);

        var result = _handler.Handle(new DeleteDriverCommand
        {
            Id = existing.Id
        });

        Assert.True(result.IsSuccess);

        _driverRepositoryMock.Verify(
            r => r.Remove(existing.Id),
            Times.Once);

        _driverRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }
}
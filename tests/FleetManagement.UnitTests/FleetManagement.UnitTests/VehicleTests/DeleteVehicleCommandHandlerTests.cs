using FleetManagement.Application.Vehicles.Commands.DeleteVehicle;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FleetManagement.UnitTests.Vehicles;

public class DeleteVehicleCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<ILogger<DeleteVehicleCommandHandler>> _loggerMock;
    private readonly DeleteVehicleCommandHandler _handler;

    public DeleteVehicleCommandHandlerTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _loggerMock = new Mock<ILogger<DeleteVehicleCommandHandler>>();

        _handler = new DeleteVehicleCommandHandler(
            _vehicleRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Fail_When_Vehicle_Not_Found()
    {
        var command = new DeleteVehicleCommand
        {
            Id = Guid.NewGuid()
        };

        _vehicleRepositoryMock
            .Setup(r => r.GetById(command.Id))
            .Returns((Vehicle?)null);

        var result = _handler.Handle(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle not found.", result.Error);
    }

    [Fact]
    public void Handle_Should_Fail_When_Vehicle_Has_Trips()
    {
        var vehicle = new Vehicle(
            Guid.NewGuid(),
            "ABC1234",
            "Fiat Uno",
            2020);

        _vehicleRepositoryMock
            .Setup(r => r.GetById(vehicle.Id))
            .Returns(vehicle);

        _vehicleRepositoryMock
            .Setup(r => r.HasTrips(vehicle.Id))
            .Returns(true);

        var result = _handler.Handle(new DeleteVehicleCommand
        {
            Id = vehicle.Id
        });

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Cannot delete vehicle because there are trips associated with this vehicle.",
            result.Error);
    }

    [Fact]
    public void Handle_Should_Succeed_When_Vehicle_Has_No_Trips()
    {
        var vehicle = new Vehicle(
            Guid.NewGuid(),
            "ABC1234",
            "Fiat Uno",
            2020);

        _vehicleRepositoryMock
            .Setup(r => r.GetById(vehicle.Id))
            .Returns(vehicle);

        _vehicleRepositoryMock
            .Setup(r => r.HasTrips(vehicle.Id))
            .Returns(false);

        var result = _handler.Handle(new DeleteVehicleCommand
        {
            Id = vehicle.Id
        });

        Assert.True(result.IsSuccess);

        _vehicleRepositoryMock.Verify(
            r => r.Remove(vehicle.Id),
            Times.Once);

        _vehicleRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }
}
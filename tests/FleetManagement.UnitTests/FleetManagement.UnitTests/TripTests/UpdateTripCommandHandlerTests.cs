using FleetManagement.Application.Trips.Commands.UpdateTrip;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FleetManagement.UnitTests.Trips;

public class UpdateTripCommandHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<IValidator<UpdateTripCommand>> _validatorMock;
    private readonly Mock<ILogger<UpdateTripCommandHandler>> _loggerMock;
    private readonly UpdateTripCommandHandler _handler;

    public UpdateTripCommandHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _validatorMock = new Mock<IValidator<UpdateTripCommand>>();
        _loggerMock = new Mock<ILogger<UpdateTripCommandHandler>>();

        _handler = new UpdateTripCommandHandler(
            _tripRepositoryMock.Object,
            _vehicleRepositoryMock.Object,
            _driverRepositoryMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Fail_When_Trip_Not_Found()
    {
        var tripId = Guid.NewGuid();
        var command = CreateValidCommand();

        SetupValidValidation();

        _tripRepositoryMock
            .Setup(r => r.GetById(tripId))
            .Returns((Trip?)null);

        var result = _handler.Handle(tripId, command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Trip not found.", result.Error);
    }

    [Fact]
    public void Handle_Should_Fail_When_Trip_Already_Started()
    {
        var tripId = Guid.NewGuid();
        var command = CreateValidCommand();

        SetupValidValidation();

        var existing = new Trip(
            tripId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddMinutes(-1),
            DateTime.UtcNow.AddDays(1));

        _tripRepositoryMock
            .Setup(r => r.GetById(tripId))
            .Returns(existing);

        var result = _handler.Handle(tripId, command);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Trip has already started and cannot be updated.",
            result.Error);
    }

    [Fact]
    public void Handle_Should_Fail_When_Vehicle_Not_Found()
    {
        var tripId = Guid.NewGuid();
        var command = CreateValidCommand();

        SetupValidValidation();

        var existing = CreateFutureTrip(tripId);

        _tripRepositoryMock
            .Setup(r => r.GetById(tripId))
            .Returns(existing);

        _vehicleRepositoryMock
            .Setup(r => r.GetById(command.VehicleId))
            .Returns((Vehicle?)null);

        var result = _handler.Handle(tripId, command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle not found.", result.Error);
    }

    [Fact]
    public void Handle_Should_Fail_When_Driver_Not_Found()
    {
        var tripId = Guid.NewGuid();
        var command = CreateValidCommand();

        SetupValidValidation();

        var existing = CreateFutureTrip(tripId);

        _tripRepositoryMock
            .Setup(r => r.GetById(tripId))
            .Returns(existing);

        _vehicleRepositoryMock
            .Setup(r => r.GetById(command.VehicleId))
            .Returns(new Vehicle(
                command.VehicleId,
                "ABC1234",
                "Civic",
                2022));

        _driverRepositoryMock
            .Setup(r => r.GetById(command.DriverId))
            .Returns((Driver?)null);

        var result = _handler.Handle(tripId, command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Driver not found.", result.Error);
    }

    [Fact]
    public void Handle_Should_Succeed_When_Trip_Is_Valid()
    {
        var tripId = Guid.NewGuid();
        var command = CreateValidCommand();

        SetupValidValidation();

        var existing = CreateFutureTrip(tripId);

        _tripRepositoryMock
            .Setup(r => r.GetById(tripId))
            .Returns(existing);

        _vehicleRepositoryMock
            .Setup(r => r.GetById(command.VehicleId))
            .Returns(new Vehicle(
                command.VehicleId,
                "XYZ9876",
                "Corolla",
                2022));

        _driverRepositoryMock
            .Setup(r => r.GetById(command.DriverId))
            .Returns(new Driver(
                command.DriverId,
                "Maria",
                "98765432101",
                DateTime.UtcNow.AddYears(1)));

        _tripRepositoryMock
            .Setup(r => r.GetTripsByVehicle(command.VehicleId))
            .Returns(new List<Trip>());

        _tripRepositoryMock
            .Setup(r => r.GetTripsByDriver(command.DriverId))
            .Returns(new List<Trip>());

        var result = _handler.Handle(tripId, command);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.VehicleId, result.Value.VehicleId);
        Assert.Equal(command.DriverId, result.Value.DriverId);

        _tripRepositoryMock.Verify(
            r => r.Update(existing),
            Times.Once);

        _tripRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }

    private static UpdateTripCommand CreateValidCommand()
    {
        return new UpdateTripCommand
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(6)
        };
    }

    private static Trip CreateFutureTrip(Guid id)
    {
        return new Trip(
            id,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(3));
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.Validate(It.IsAny<UpdateTripCommand>()))
            .Returns(new FluentValidation.Results.ValidationResult());
    }
}
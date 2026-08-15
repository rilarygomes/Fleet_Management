using FleetManagement.Application.Trips.Commands.CreateTrip;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FleetManagement.UnitTests.Trips;

public class CreateTripCommandHandlerTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<IValidator<CreateTripCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateTripCommandHandler>> _loggerMock;
    private readonly CreateTripCommandHandler _handler;

    public CreateTripCommandHandlerTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _validatorMock = new Mock<IValidator<CreateTripCommand>>();
        _loggerMock = new Mock<ILogger<CreateTripCommandHandler>>();

        _handler = new CreateTripCommandHandler(
            _tripRepositoryMock.Object,
            _vehicleRepositoryMock.Object,
            _driverRepositoryMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Fail_When_Vehicle_Not_Found()
    {
        var command = CreateValidCommand();

        SetupValidValidation();

        _vehicleRepositoryMock
            .Setup(r => r.GetById(command.VehicleId))
            .Returns((Vehicle?)null);

        var result = _handler.Handle(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle not found.", result.Error);
    }

    [Fact]
    public void Handle_Should_Fail_When_Driver_Not_Found()
    {
        var command = CreateValidCommand();

        SetupValidValidation();

        _vehicleRepositoryMock
            .Setup(r => r.GetById(command.VehicleId))
            .Returns(new Vehicle(
                command.VehicleId,
                "ABC1234",
                "Fiat Uno",
                2020));

        _driverRepositoryMock
            .Setup(r => r.GetById(command.DriverId))
            .Returns((Driver?)null);

        var result = _handler.Handle(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Driver not found.", result.Error);
    }

    [Fact]
    public void Handle_Should_Fail_When_Vehicle_Has_Conflicting_Trip()
    {
        var command = CreateValidCommand();

        SetupValidValidation();
        SetupExistingVehicleAndDriver(command);

        var conflictingTrip = new Trip(
            Guid.NewGuid(),
            command.VehicleId,
            Guid.NewGuid(),
            command.StartDate.AddHours(-1),
            command.EndDate.AddHours(-1));

        _tripRepositoryMock
            .Setup(r => r.GetTripsByVehicle(command.VehicleId))
            .Returns(new List<Trip> { conflictingTrip });

        var result = _handler.Handle(command);

        Assert.False(result.IsSuccess);
        Assert.Contains("Vehicle is already assigned", result.Error);
    }

    [Fact]
    public void Handle_Should_Fail_When_Driver_Has_Conflicting_Trip()
    {
        var command = CreateValidCommand();

        SetupValidValidation();
        SetupExistingVehicleAndDriver(command);

        _tripRepositoryMock
            .Setup(r => r.GetTripsByVehicle(command.VehicleId))
            .Returns(new List<Trip>());

        var conflictingTrip = new Trip(
            Guid.NewGuid(),
            Guid.NewGuid(),
            command.DriverId,
            command.StartDate.AddHours(-1),
            command.EndDate.AddHours(-1));

        _tripRepositoryMock
            .Setup(r => r.GetTripsByDriver(command.DriverId))
            .Returns(new List<Trip> { conflictingTrip });

        var result = _handler.Handle(command);

        Assert.False(result.IsSuccess);
        Assert.Contains("Driver is already assigned", result.Error);
    }

    [Fact]
    public void Handle_Should_Succeed_When_Trip_Is_Valid()
    {
        var command = CreateValidCommand();

        SetupValidValidation();
        SetupExistingVehicleAndDriver(command);

        _tripRepositoryMock
            .Setup(r => r.GetTripsByVehicle(command.VehicleId))
            .Returns(new List<Trip>());

        _tripRepositoryMock
            .Setup(r => r.GetTripsByDriver(command.DriverId))
            .Returns(new List<Trip>());

        var result = _handler.Handle(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.VehicleId, result.Value.VehicleId);
        Assert.Equal(command.DriverId, result.Value.DriverId);
        Assert.Equal(command.StartDate, result.Value.StartDate);
        Assert.Equal(command.EndDate, result.Value.EndDate);

        _tripRepositoryMock.Verify(
            r => r.Add(It.IsAny<Trip>()),
            Times.Once);

        _tripRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }

    private static CreateTripCommand CreateValidCommand()
    {
        return new CreateTripCommand
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(3)
        };
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.Validate(
                It.IsAny<ValidationContext<CreateTripCommand>>()))
            .Returns(new FluentValidation.Results.ValidationResult());
    }

    private void SetupExistingVehicleAndDriver(CreateTripCommand command)
    {
        _vehicleRepositoryMock
            .Setup(r => r.GetById(command.VehicleId))
            .Returns(new Vehicle(
                command.VehicleId,
                "ABC1234",
                "Fiat Uno",
                2020));

        _driverRepositoryMock
            .Setup(r => r.GetById(command.DriverId))
            .Returns(new Driver(
                command.DriverId,
                "Carlos",
                "12345678901",
                DateTime.UtcNow.AddYears(1)));
    }
}
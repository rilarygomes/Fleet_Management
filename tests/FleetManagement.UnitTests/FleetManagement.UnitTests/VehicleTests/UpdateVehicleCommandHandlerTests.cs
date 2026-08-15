using FleetManagement.Application.Vehicles.Commands.UpdateVehicle;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FleetManagement.UnitTests.Vehicles;

public class UpdateVehicleCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IValidator<UpdateVehicleCommand>> _validatorMock;
    private readonly Mock<ILogger<UpdateVehicleCommandHandler>> _loggerMock;
    private readonly UpdateVehicleCommandHandler _handler;

    public UpdateVehicleCommandHandlerTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _validatorMock = new Mock<IValidator<UpdateVehicleCommand>>();
        _loggerMock = new Mock<ILogger<UpdateVehicleCommandHandler>>();

        _handler = new UpdateVehicleCommandHandler(
            _vehicleRepositoryMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Fail_When_Vehicle_Not_Found()
    {
        var command = CreateValidCommand();

        SetupValidValidation();

        _vehicleRepositoryMock
            .Setup(r => r.GetById(command.Id))
            .Returns((Vehicle?)null);

        var result = _handler.Handle(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle not found.", result.Error);
    }

    [Fact]
    public void Handle_Should_Fail_When_LicensePlate_Belongs_To_Another_Vehicle()
    {
        var existing = new Vehicle(
            Guid.NewGuid(),
            "ABC1234",
            "Fiat Uno",
            2020);

        var duplicate = new Vehicle(
            Guid.NewGuid(),
            "XYZ9876",
            "Toyota Corolla",
            2022);

        var command = new UpdateVehicleCommand
        {
            Id = existing.Id,
            LicensePlate = duplicate.LicensePlate,
            Model = "Fiat Uno Updated",
            Year = 2021
        };

        SetupValidValidation();

        _vehicleRepositoryMock
            .Setup(r => r.GetById(command.Id))
            .Returns(existing);

        _vehicleRepositoryMock
            .Setup(r => r.GetByLicensePlate(command.LicensePlate))
            .Returns(duplicate);

        var result = _handler.Handle(command);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Another vehicle already uses this license plate.",
            result.Error);
    }

    [Fact]
    public void Handle_Should_Succeed_When_Vehicle_Exists()
    {
        var existing = new Vehicle(
            Guid.NewGuid(),
            "ABC1234",
            "Fiat Uno",
            2020);

        var command = new UpdateVehicleCommand
        {
            Id = existing.Id,
            LicensePlate = "XYZ9876",
            Model = "Toyota Corolla",
            Year = 2022
        };

        SetupValidValidation();

        _vehicleRepositoryMock
            .Setup(r => r.GetById(command.Id))
            .Returns(existing);

        _vehicleRepositoryMock
            .Setup(r => r.GetByLicensePlate(command.LicensePlate))
            .Returns((Vehicle?)null);

        var result = _handler.Handle(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.LicensePlate, result.Value.LicensePlate);
        Assert.Equal(command.Model, result.Value.Model);

        _vehicleRepositoryMock.Verify(
            r => r.Update(existing),
            Times.Once);

        _vehicleRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }

    private static UpdateVehicleCommand CreateValidCommand()
    {
        return new UpdateVehicleCommand
        {
            Id = Guid.NewGuid(),
            LicensePlate = "XYZ9876",
            Model = "Toyota Corolla",
            Year = 2022
        };
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.Validate(
                It.IsAny<ValidationContext<UpdateVehicleCommand>>()))
            .Returns(new FluentValidation.Results.ValidationResult());
    }
}
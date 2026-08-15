using FleetManagement.Application.Vehicles.Commands.CreateVehicle;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FleetManagement.UnitTests.Vehicles;

public class CreateVehicleCommandHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IValidator<CreateVehicleCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateVehicleCommandHandler>> _loggerMock;
    private readonly CreateVehicleCommandHandler _handler;

    public CreateVehicleCommandHandlerTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _validatorMock = new Mock<IValidator<CreateVehicleCommand>>();
        _loggerMock = new Mock<ILogger<CreateVehicleCommandHandler>>();

        _handler = new CreateVehicleCommandHandler(
            _vehicleRepositoryMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Fail_When_Vehicle_With_Same_LicensePlate_Exists()
    {
        var command = new CreateVehicleCommand
        {
            LicensePlate = "ABC1234",
            Model = "Fiat Uno",
            Year = 2020
        };

        SetupValidValidation();

        _vehicleRepositoryMock
            .Setup(r => r.GetByLicensePlate(command.LicensePlate))
            .Returns(new Vehicle(
                Guid.NewGuid(),
                command.LicensePlate,
                command.Model,
                command.Year));

        var result = _handler.Handle(command);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "A vehicle with this license plate already exists.",
            result.Error);
    }

    [Fact]
    public void Handle_Should_Succeed_When_Vehicle_Is_Valid_And_Not_Duplicated()
    {
        var command = new CreateVehicleCommand
        {
            LicensePlate = "XYZ9876",
            Model = "Toyota Corolla",
            Year = 2022
        };

        SetupValidValidation();

        _vehicleRepositoryMock
            .Setup(r => r.GetByLicensePlate(command.LicensePlate))
            .Returns((Vehicle?)null);

        var result = _handler.Handle(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.LicensePlate, result.Value.LicensePlate);
        Assert.Equal(command.Model, result.Value.Model);
        Assert.Equal(command.Year, result.Value.Year);

        _vehicleRepositoryMock.Verify(
            r => r.Add(It.IsAny<Vehicle>()),
            Times.Once);

        _vehicleRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.Validate(
                It.IsAny<ValidationContext<CreateVehicleCommand>>()))
            .Returns(new FluentValidation.Results.ValidationResult());
    }
}
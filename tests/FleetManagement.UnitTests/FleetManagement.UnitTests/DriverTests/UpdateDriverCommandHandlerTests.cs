using FleetManagement.Application.Drivers.Commands.UpdateDriver;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class UpdateDriverCommandHandlerTests
{
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<IValidator<UpdateDriverCommand>> _validatorMock;
    private readonly Mock<ILogger<UpdateDriverCommandHandler>> _loggerMock;
    private readonly UpdateDriverCommandHandler _handler;

    public UpdateDriverCommandHandlerTests()
    {
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _validatorMock = new Mock<IValidator<UpdateDriverCommand>>();
        _loggerMock = new Mock<ILogger<UpdateDriverCommandHandler>>();

        _handler = new UpdateDriverCommandHandler(
            _driverRepositoryMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Fail_When_Driver_Not_Found()
    {
        var driverId = Guid.NewGuid();

        var command = new UpdateDriverCommand
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(1)
        };

        SetupValidValidation();

        _driverRepositoryMock
            .Setup(r => r.GetById(driverId))
            .Returns((Driver?)null);

        var result = _handler.Handle(driverId, command);

        Assert.False(result.IsSuccess);
        Assert.Equal("Driver not found.", result.Error);
    }

    [Fact]
    public void Handle_Should_Fail_When_LicenseNumber_Belongs_To_Another_Driver()
    {
        var driverId = Guid.NewGuid();

        var existing = new Driver(
            driverId,
            "Carlos",
            "12345678901",
            DateTime.UtcNow.AddYears(1));

        var duplicate = new Driver(
            Guid.NewGuid(),
            "Maria",
            "98765432101",
            DateTime.UtcNow.AddYears(1));

        var command = new UpdateDriverCommand
        {
            Name = "Carlos Updated",
            LicenseNumber = duplicate.LicenseNumber,
            LicenseExpirationDate = DateTime.UtcNow.AddYears(2)
        };

        SetupValidValidation();

        _driverRepositoryMock
            .Setup(r => r.GetById(driverId))
            .Returns(existing);

        _driverRepositoryMock
            .Setup(r => r.GetByLicenseNumber(command.LicenseNumber))
            .Returns(duplicate);

        var result = _handler.Handle(driverId, command);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "Another driver already uses this license number.",
            result.Error);
    }

    [Fact]
    public void Handle_Should_Succeed_When_Driver_Exists()
    {
        var existing = new Driver(
            Guid.NewGuid(),
            "Maria",
            "98765432101",
            DateTime.UtcNow.AddYears(1));

        var command = new UpdateDriverCommand
        {
            Name = "Maria Updated",
            LicenseNumber = existing.LicenseNumber,
            LicenseExpirationDate = DateTime.UtcNow.AddYears(2)
        };

        SetupValidValidation();

        _driverRepositoryMock
            .Setup(r => r.GetById(existing.Id))
            .Returns(existing);

        _driverRepositoryMock
            .Setup(r => r.GetByLicenseNumber(command.LicenseNumber))
            .Returns(existing);

        var result = _handler.Handle(existing.Id, command);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.Name, result.Value!.Name);

        _driverRepositoryMock.Verify(
            r => r.Update(existing),
            Times.Once);

        _driverRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.Validate(It.IsAny<UpdateDriverCommand>()))
            .Returns(new FluentValidation.Results.ValidationResult());
    }
}
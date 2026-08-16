using FleetManagement.Application.Drivers.Commands.CreateDriver;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class CreateDriverCommandHandlerTests
{
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<IValidator<CreateDriverCommand>> _validatorMock;
    private readonly Mock<ILogger<CreateDriverCommandHandler>> _loggerMock;
    private readonly CreateDriverCommandHandler _handler;

    public CreateDriverCommandHandlerTests()
    {
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _validatorMock = new Mock<IValidator<CreateDriverCommand>>();
        _loggerMock = new Mock<ILogger<CreateDriverCommandHandler>>();

        _handler = new CreateDriverCommandHandler(
            _driverRepositoryMock.Object,
            _validatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Fail_When_Driver_With_Same_LicenseNumber_Exists()
    {
        var command = new CreateDriverCommand
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        SetupValidValidation();

        _driverRepositoryMock
            .Setup(r => r.GetByLicenseNumber(command.LicenseNumber))
            .Returns(new Driver(
                Guid.NewGuid(),
                "Carlos",
                command.LicenseNumber,
                DateTime.Today.AddYears(1)));

        var result = _handler.Handle(command);

        Assert.False(result.IsSuccess);
        Assert.Equal(
            "A driver with this license number already exists.",
            result.Error);
    }

    [Fact]
    public void Handle_Should_Succeed_When_Driver_Is_Valid_And_Not_Duplicated()
    {
        var command = new CreateDriverCommand
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        SetupValidValidation();

        _driverRepositoryMock
            .Setup(r => r.GetByLicenseNumber(command.LicenseNumber))
            .Returns((Driver?)null);

        var result = _handler.Handle(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.Name, result.Value!.Name);
        Assert.Equal(command.LicenseNumber, result.Value.LicenseNumber);

        _driverRepositoryMock.Verify(
            r => r.Add(It.IsAny<Driver>()),
            Times.Once);

        _driverRepositoryMock.Verify(
            r => r.SaveChanges(),
            Times.Once);
    }

    private void SetupValidValidation()
    {
        _validatorMock
            .Setup(v => v.Validate(It.IsAny<CreateDriverCommand>()))
            .Returns(new FluentValidation.Results.ValidationResult());
    }
}
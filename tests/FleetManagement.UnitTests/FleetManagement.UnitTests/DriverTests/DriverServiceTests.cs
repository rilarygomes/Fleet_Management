using FleetManagement.Application.DTOs;
using FleetManagement.Application.Services;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class DriverServiceTests
{
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<IValidator<CreateDriverDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateDriverDto>> _updateValidatorMock;
    private readonly Mock<ILogger<DriverService>> _loggerMock;
    private readonly DriverService _driverService;

    public DriverServiceTests()
    {
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _createValidatorMock = new Mock<IValidator<CreateDriverDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateDriverDto>>();
        _loggerMock = new Mock<ILogger<DriverService>>();

        _driverService = new DriverService(
            _driverRepositoryMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _loggerMock.Object
        );
    }

    // --- ADD ---
    [Fact]
    public void Add_Should_Fail_When_Driver_With_Same_LicenseNumber_Exists()
    {
        var dto = new CreateDriverDto
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        _createValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _driverRepositoryMock.Setup(r => r.GetByLicenseNumber(dto.LicenseNumber))
            .Returns(new Driver(Guid.NewGuid(), "Carlos", "12345678901", DateTime.Today.AddYears(1)));

        var result = _driverService.Add(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("A driver with this license number already exists.", result.Error);
    }

    [Fact]
    public void Add_Should_Succeed_When_Driver_Is_Valid_And_Not_Duplicated()
    {
        var dto = new CreateDriverDto
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.Today.AddYears(1)
        };

        _createValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _driverRepositoryMock.Setup(r => r.GetByLicenseNumber(dto.LicenseNumber))
            .Returns((Driver)null!);

        var result = _driverService.Add(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto.Name, result.Value.Name);
        Assert.Equal(dto.LicenseNumber, result.Value.LicenseNumber);
    }

    // --- UPDATE ---
    [Fact]
    public void Update_Should_Fail_When_Driver_Not_Found()
    {
        var dto = new UpdateDriverDto
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(1)
        };

        _updateValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _driverRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>()))
            .Returns((Driver)null!);

        var result = _driverService.Update(Guid.NewGuid(), dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Driver not found.", result.Error);
    }

    [Fact]
    public void Update_Should_Succeed_When_Driver_Exists()
    {
        var existing = new Driver(Guid.NewGuid(), "Maria", "98765432101", DateTime.UtcNow.AddYears(1));
        var dto = new UpdateDriverDto
        {
            Name = "Maria Updated",
            LicenseNumber = "98765432101",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(2)
        };

        _updateValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _driverRepositoryMock.Setup(r => r.GetById(existing.Id)).Returns(existing);

        var result = _driverService.Update(existing.Id, dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto.Name, result.Value.Name);
    }

    // --- REMOVE ---
    [Fact]
    public void Remove_Should_Fail_When_Driver_Not_Found()
    {
        _driverRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>()))
            .Returns((Driver)null!);

        var result = _driverService.Remove(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Driver not found.", result.Error);
    }

    [Fact]
    public void Remove_Should_Fail_When_Driver_Has_Trips()
    {
        var existing = new Driver(Guid.NewGuid(), "Carlos", "12345678901", DateTime.UtcNow.AddYears(1));
        _driverRepositoryMock.Setup(r => r.GetById(existing.Id)).Returns(existing);
        _driverRepositoryMock.Setup(r => r.HasTrips(existing.Id)).Returns(true);

        var result = _driverService.Remove(existing.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot delete driver because there are trips associated with this driver.", result.Error);
    }

    [Fact]
    public void Remove_Should_Succeed_When_Driver_Has_No_Trips()
    {
        var existing = new Driver(Guid.NewGuid(), "Carlos", "12345678901", DateTime.UtcNow.AddYears(1));
        _driverRepositoryMock.Setup(r => r.GetById(existing.Id)).Returns(existing);
        _driverRepositoryMock.Setup(r => r.HasTrips(existing.Id)).Returns(false);

        var result = _driverService.Remove(existing.Id);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    // --- GETBYID ---
    [Fact]
    public void GetById_Should_Return_Null_When_Driver_Not_Found()
    {
        _driverRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>()))
            .Returns((Driver)null!);

        var result = _driverService.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public void GetById_Should_Return_DriverDto_When_Found()
    {
        var existing = new Driver(Guid.NewGuid(), "Carlos", "12345678901", DateTime.UtcNow.AddYears(1));
        _driverRepositoryMock.Setup(r => r.GetById(existing.Id)).Returns(existing);

        var result = _driverService.GetById(existing.Id);

        Assert.NotNull(result);
        Assert.Equal(existing.Name, result!.Name);
    }

    // --- GETALL ---
    [Fact]
    public void GetAll_Should_Return_List_Of_Drivers()
    {
        var drivers = new List<Driver>
        {
            new Driver(Guid.NewGuid(), "Carlos", "12345678901", DateTime.UtcNow.AddYears(1)),
            new Driver(Guid.NewGuid(), "Maria", "98765432101", DateTime.UtcNow.AddYears(1))
        };

        _driverRepositoryMock.Setup(r => r.GetAll()).Returns(drivers);

        var result = _driverService.GetAll().ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Name == "Carlos");
        Assert.Contains(result, d => d.Name == "Maria");
    }
}

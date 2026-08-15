using FleetManagement.Application.DTOs;
using FleetManagement.Application.Services;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class VehicleServiceTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IValidator<CreateVehicleDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateVehicleDto>> _updateValidatorMock;
    private readonly Mock<ILogger<VehicleService>> _loggerMock;
    private readonly VehicleService _vehicleService;

    public VehicleServiceTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _createValidatorMock = new Mock<IValidator<CreateVehicleDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateVehicleDto>>();
        _loggerMock = new Mock<ILogger<VehicleService>>();

        _vehicleService = new VehicleService(
            _vehicleRepositoryMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _loggerMock.Object
        );
    }

    // --- GETALL ---
    [Fact]
    public void GetAll_Should_Return_List_Of_Vehicles()
    {
        var vehicles = new List<Vehicle>
        {
            new Vehicle(Guid.NewGuid(), "ABC1234", "Fiat Uno", 2020),
            new Vehicle(Guid.NewGuid(), "XYZ9876", "Toyota Corolla", 2022)
        };

        _vehicleRepositoryMock.Setup(r => r.GetAll()).Returns(vehicles);

        var result = _vehicleService.GetAll().ToList();

        Assert.Equal(2, result.Count);
    }

    // --- GETBYID ---
    [Fact]
    public void GetById_Should_Return_Null_When_Vehicle_Not_Found()
    {
        _vehicleRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Vehicle)null!);

        var result = _vehicleService.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public void GetById_Should_Return_VehicleDto_When_Found()
    {
        var vehicle = new Vehicle(Guid.NewGuid(), "ABC1234", "Fiat Uno", 2020);
        _vehicleRepositoryMock.Setup(r => r.GetById(vehicle.Id)).Returns(vehicle);

        var result = _vehicleService.GetById(vehicle.Id);

        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result!.Id);
    }

    // --- ADD ---
    [Fact]
    public void Add_Should_Fail_When_Vehicle_With_Same_LicensePlate_Exists()
    {
        var dto = new CreateVehicleDto
        {
            LicensePlate = "ABC1234",
            Model = "Fiat Uno",
            Year = 2020
        };

        _createValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _vehicleRepositoryMock.Setup(r => r.GetByLicensePlate(dto.LicensePlate))
            .Returns(new Vehicle(Guid.NewGuid(), "ABC1234", "Fiat Uno", 2020));

        var result = _vehicleService.Add(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("A vehicle with this license plate already exists.", result.Error);
    }

    [Fact]
    public void Add_Should_Succeed_When_Vehicle_Is_Valid_And_Not_Duplicated()
    {
        var dto = new CreateVehicleDto
        {
            LicensePlate = "XYZ9876",
            Model = "Toyota Corolla",
            Year = 2022
        };

        _createValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _vehicleRepositoryMock.Setup(r => r.GetByLicensePlate(dto.LicensePlate)).Returns((Vehicle)null!);

        var result = _vehicleService.Add(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto.LicensePlate, result.Value.LicensePlate);
        Assert.Equal(dto.Model, result.Value.Model);
    }

    // --- UPDATE ---
    [Fact]
    public void Update_Should_Fail_When_Vehicle_Not_Found()
    {
        var dto = new UpdateVehicleDto
        {
            LicensePlate = "XYZ9876",
            Model = "Toyota Corolla",
            Year = 2022
        };

        _updateValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _vehicleRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Vehicle)null!);

        var result = _vehicleService.Update(Guid.NewGuid(), dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle not found.", result.Error);
    }

    [Fact]
    public void Update_Should_Succeed_When_Vehicle_Exists()
    {
        var existing = new Vehicle(Guid.NewGuid(), "ABC1234", "Fiat Uno", 2020);
        var dto = new UpdateVehicleDto
        {
            LicensePlate = "XYZ9876",
            Model = "Toyota Corolla",
            Year = 2022
        };

        _updateValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _vehicleRepositoryMock.Setup(r => r.GetById(existing.Id)).Returns(existing);

        var result = _vehicleService.Update(existing.Id, dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto.LicensePlate, result.Value.LicensePlate);
        Assert.Equal(dto.Model, result.Value.Model);
    }

    // --- REMOVE ---
    [Fact]
    public void Remove_Should_Fail_When_Vehicle_Not_Found()
    {
        _vehicleRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Vehicle)null!);

        var result = _vehicleService.Remove(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle not found.", result.Error);
    }

    [Fact]
    public void Remove_Should_Fail_When_Vehicle_Has_Trips()
    {
        var existing = new Vehicle(Guid.NewGuid(), "ABC1234", "Fiat Uno", 2020);
        _vehicleRepositoryMock.Setup(r => r.GetById(existing.Id)).Returns(existing);
        _vehicleRepositoryMock.Setup(r => r.HasTrips(existing.Id)).Returns(true);

        var result = _vehicleService.Remove(existing.Id);

        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot delete vehicle because there are trips associated with this vehicle.", result.Error);
    }

    [Fact]
    public void Remove_Should_Succeed_When_Vehicle_Has_No_Trips()
    {
        var existing = new Vehicle(Guid.NewGuid(), "ABC1234", "Fiat Uno", 2020);
        _vehicleRepositoryMock.Setup(r => r.GetById(existing.Id)).Returns(existing);
        _vehicleRepositoryMock.Setup(r => r.HasTrips(existing.Id)).Returns(false);

        var result = _vehicleService.Remove(existing.Id);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }
}

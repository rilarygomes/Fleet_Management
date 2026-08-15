using FleetManagement.Application.DTOs;
using FleetManagement.Application.Services;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;

public class TripServiceTests
{
    private readonly Mock<ITripRepository> _tripRepositoryMock;
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<IValidator<CreateTripDto>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateTripDto>> _updateValidatorMock;
    private readonly Mock<ILogger<TripService>> _loggerMock;
    private readonly TripService _tripService;

    public TripServiceTests()
    {
        _tripRepositoryMock = new Mock<ITripRepository>();
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _createValidatorMock = new Mock<IValidator<CreateTripDto>>();
        _updateValidatorMock = new Mock<IValidator<UpdateTripDto>>();
        _loggerMock = new Mock<ILogger<TripService>>();

        _tripService = new TripService(
            _tripRepositoryMock.Object,
            _vehicleRepositoryMock.Object,
            _driverRepositoryMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _loggerMock.Object
        );
    }

    // --- GETALL ---
    [Fact]
    public void GetAll_Should_Return_List_Of_Trips()
    {
        var trips = new List<Trip>
        {
            new Trip(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2)),
            new Trip(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(4))
        };

        _tripRepositoryMock.Setup(r => r.GetAll()).Returns(trips);

        var result = _tripService.GetAll().ToList();

        Assert.Equal(2, result.Count);
    }

    // --- GETBYID ---
    [Fact]
    public void GetById_Should_Return_Null_When_Trip_Not_Found()
    {
        _tripRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Trip)null!);

        var result = _tripService.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public void GetById_Should_Return_TripDto_When_Found()
    {
        var trip = new Trip(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));
        _tripRepositoryMock.Setup(r => r.GetById(trip.Id)).Returns(trip);

        var result = _tripService.GetById(trip.Id);

        Assert.NotNull(result);
        Assert.Equal(trip.Id, result!.Id);
    }

    // --- ADD ---
    [Fact]
    public void Add_Should_Fail_When_Vehicle_Not_Found()
    {
        var dto = new CreateTripDto
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        };

        _createValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _vehicleRepositoryMock.Setup(r => r.GetById(dto.VehicleId)).Returns((Vehicle)null!);

        var result = _tripService.Add(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle not found.", result.Error);
    }

    [Fact]
    public void Add_Should_Fail_When_Driver_Not_Found()
    {
        var dto = new CreateTripDto
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        };

        _createValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _vehicleRepositoryMock.Setup(r => r.GetById(dto.VehicleId))
            .Returns(new Vehicle(dto.VehicleId, "ABC1234", "Fiat Uno", 2020));
        _driverRepositoryMock.Setup(r => r.GetById(dto.DriverId)).Returns((Driver)null!);

        var result = _tripService.Add(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Driver not found.", result.Error);
    }

    [Fact]
    public void Add_Should_Succeed_When_All_Valid()
    {
        var dto = new CreateTripDto
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        };

        _createValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _vehicleRepositoryMock.Setup(r => r.GetById(dto.VehicleId))
            .Returns(new Vehicle(dto.VehicleId, "ABC1234", "Fiat Uno", 2020));
        _driverRepositoryMock.Setup(r => r.GetById(dto.DriverId))
            .Returns(new Driver(dto.DriverId, "Carlos", "12345678901", DateTime.UtcNow.AddYears(1)));
        _tripRepositoryMock.Setup(r => r.GetTripsByVehicle(dto.VehicleId)).Returns(new List<Trip>());
        _tripRepositoryMock.Setup(r => r.GetTripsByDriver(dto.DriverId)).Returns(new List<Trip>());

        var result = _tripService.Add(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto.VehicleId, result.Value.VehicleId);
        Assert.Equal(dto.DriverId, result.Value.DriverId);
    }

    // --- UPDATE ---
    [Fact]
    public void Update_Should_Fail_When_Trip_Not_Found()
    {
        var dto = new UpdateTripDto
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        };

        _updateValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _tripRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Trip)null!);

        var result = _tripService.Update(Guid.NewGuid(), dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Trip not found.", result.Error);
    }

    [Fact]
    public void Update_Should_Fail_When_Trip_Already_Started()
    {
        var existing = new Trip(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        var dto = new UpdateTripDto
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(3)
        };

        _updateValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _tripRepositoryMock.Setup(r => r.GetById(existing.Id)).Returns(existing);

        var result = _tripService.Update(existing.Id, dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Trip has already started and cannot be updated.", result.Error);
    }

    [Fact]
    public void Update_Should_Succeed_When_All_Valid()
    {
        var existing = new Trip(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(3));

        var dto = new UpdateTripDto
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(4),
            EndDate = DateTime.UtcNow.AddDays(5)
        };

        _updateValidatorMock.Setup(v => v.Validate(dto))
            .Returns(new FluentValidation.Results.ValidationResult());

        _tripRepositoryMock.Setup(r => r.GetById(existing.Id)).Returns(existing);
        _vehicleRepositoryMock.Setup(r => r.GetById(dto.VehicleId))
            .Returns(new Vehicle(dto.VehicleId, "XYZ9876", "Toyota Corolla", 2022));
        _driverRepositoryMock.Setup(r => r.GetById(dto.DriverId))
            .Returns(new Driver(dto.DriverId, "Maria", "98765432101", DateTime.UtcNow.AddYears(1)));
        _tripRepositoryMock.Setup(r => r.GetTripsByVehicle(dto.VehicleId)).Returns(new List<Trip>());
        _tripRepositoryMock.Setup(r => r.GetTripsByDriver(dto.DriverId)).Returns(new List<Trip>());

        var result = _tripService.Update(existing.Id, dto);

        Assert.True(result.IsSuccess);
        Assert.Equal(dto.VehicleId, result.Value.VehicleId);
        Assert.Equal(dto.DriverId, result.Value.DriverId);
    }

    // --- REMOVE ---
    [Fact]
    public void Remove_Should_Fail_When_Trip_Not_Found()
    {
        _tripRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((Trip)null!);

        var result = _tripService.Remove(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Trip not found.", result.Error);
    }

    [Fact]
    public void Remove_Should_Succeed_When_Trip_Exists()
    {
        var existing = new Trip(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2)
        );

        _tripRepositoryMock.Setup(r => r.GetById(existing.Id)).Returns(existing);

        var result = _tripService.Remove(existing.Id);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }
}

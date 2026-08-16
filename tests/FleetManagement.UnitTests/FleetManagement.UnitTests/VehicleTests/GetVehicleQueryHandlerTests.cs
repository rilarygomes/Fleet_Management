using FleetManagement.Application.Vehicles.GetVehicle;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FleetManagement.UnitTests.Vehicles;

public class GetVehicleQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<ILogger<GetVehicleQueryHandler>> _loggerMock;
    private readonly GetVehicleQueryHandler _handler;

    public GetVehicleQueryHandlerTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _loggerMock = new Mock<ILogger<GetVehicleQueryHandler>>();

        _handler = new GetVehicleQueryHandler(
            _vehicleRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Return_Null_When_Vehicle_Not_Found()
    {
        var query = new GetVehicleQuery
        {
            Id = Guid.NewGuid()
        };

        _vehicleRepositoryMock
            .Setup(r => r.GetById(query.Id))
            .Returns((Vehicle?)null);

        var result = _handler.Handle(query);

        Assert.Null(result);
    }

    [Fact]
    public void Handle_Should_Return_Vehicle_When_Found()
    {
        var vehicle = new Vehicle(
            Guid.NewGuid(),
            "ABC1234",
            "Fiat Uno",
            2020);

        _vehicleRepositoryMock
            .Setup(r => r.GetById(vehicle.Id))
            .Returns(vehicle);

        var result = _handler.Handle(new GetVehicleQuery
        {
            Id = vehicle.Id
        });

        Assert.NotNull(result);
        Assert.Equal(vehicle.Id, result!.Id);
        Assert.Equal(vehicle.LicensePlate, result.LicensePlate);
        Assert.Equal(vehicle.Model, result.Model);
        Assert.Equal(vehicle.Year, result.Year);
    }
}
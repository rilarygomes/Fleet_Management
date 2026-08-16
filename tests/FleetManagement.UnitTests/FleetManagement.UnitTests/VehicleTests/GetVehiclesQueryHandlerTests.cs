using FleetManagement.Application.Vehicles.GetVehicles;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FleetManagement.UnitTests.Vehicles;

public class GetVehiclesQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepositoryMock;
    private readonly Mock<ILogger<GetVehiclesQueryHandler>> _loggerMock;
    private readonly GetVehiclesQueryHandler _handler;

    public GetVehiclesQueryHandlerTests()
    {
        _vehicleRepositoryMock = new Mock<IVehicleRepository>();
        _loggerMock = new Mock<ILogger<GetVehiclesQueryHandler>>();

        _handler = new GetVehiclesQueryHandler(
            _vehicleRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Return_All_Vehicles_When_LicensePlate_Filter_Is_Not_Provided()
    {
        var vehicles = CreateVehicles();

        _vehicleRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(vehicles);

        var result = _handler
            .Handle(new GetVehiclesQuery())
            .ToList();

        Assert.Equal(2, result.Count);

        _vehicleRepositoryMock.Verify(
            r => r.GetAll(),
            Times.Once);
    }

    [Fact]
    public void Handle_Should_Filter_Vehicles_By_LicensePlate()
    {
        var vehicles = CreateVehicles();

        _vehicleRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(vehicles);

        var result = _handler
            .Handle(new GetVehiclesQuery
            {
                LicensePlate = "ABC"
            })
            .ToList();

        Assert.Single(result);
        Assert.Equal("ABC1234", result[0].LicensePlate);
    }

    private static List<Vehicle> CreateVehicles()
    {
        return new List<Vehicle>
        {
            new(
                Guid.NewGuid(),
                "ABC1234",
                "Fiat Uno",
                2020),

            new(
                Guid.NewGuid(),
                "XYZ9876",
                "Toyota Corolla",
                2022)
        };
    }
}
using FleetManagement.Application.Drivers.GetDrivers;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class GetDriversQueryHandlerTests
{
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<ILogger<GetDriversQueryHandler>> _loggerMock;
    private readonly GetDriversQueryHandler _handler;

    public GetDriversQueryHandlerTests()
    {
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _loggerMock = new Mock<ILogger<GetDriversQueryHandler>>();

        _handler = new GetDriversQueryHandler(
            _driverRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Return_All_Drivers_When_Name_Filter_Is_Not_Provided()
    {
        var drivers = new List<Driver>
        {
            new(
                Guid.NewGuid(),
                "Carlos",
                "12345678901",
                DateTime.UtcNow.AddYears(1)),

            new(
                Guid.NewGuid(),
                "Maria",
                "98765432101",
                DateTime.UtcNow.AddYears(1))
        };

        _driverRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(drivers);

        var result = _handler
            .Handle(new GetDriversQuery())
            .ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.Name == "Carlos");
        Assert.Contains(result, d => d.Name == "Maria");
    }

    [Fact]
    public void Handle_Should_Filter_Drivers_By_Name()
    {
        var drivers = new List<Driver>
        {
            new(
                Guid.NewGuid(),
                "Carlos",
                "12345678901",
                DateTime.UtcNow.AddYears(1)),

            new(
                Guid.NewGuid(),
                "Maria",
                "98765432101",
                DateTime.UtcNow.AddYears(1))
        };

        _driverRepositoryMock
            .Setup(r => r.GetAll())
            .Returns(drivers);

        var result = _handler
            .Handle(new GetDriversQuery
            {
                Name = "car"
            })
            .ToList();

        Assert.Single(result);
        Assert.Equal("Carlos", result[0].Name);
    }
}
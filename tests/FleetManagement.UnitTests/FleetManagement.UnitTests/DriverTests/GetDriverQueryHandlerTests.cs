using FleetManagement.Application.Drivers.GetDriver;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class GetDriverQueryHandlerTests
{
    private readonly Mock<IDriverRepository> _driverRepositoryMock;
    private readonly Mock<ILogger<GetDriverQueryHandler>> _loggerMock;
    private readonly GetDriverQueryHandler _handler;

    public GetDriverQueryHandlerTests()
    {
        _driverRepositoryMock = new Mock<IDriverRepository>();
        _loggerMock = new Mock<ILogger<GetDriverQueryHandler>>();

        _handler = new GetDriverQueryHandler(
            _driverRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Handle_Should_Return_Null_When_Driver_Not_Found()
    {
        var query = new GetDriverQuery
        {
            Id = Guid.NewGuid()
        };

        _driverRepositoryMock
            .Setup(r => r.GetById(query.Id))
            .Returns((Driver?)null);

        var result = _handler.Handle(query);

        Assert.Null(result);
    }

    [Fact]
    public void Handle_Should_Return_Driver_When_Found()
    {
        var existing = new Driver(
            Guid.NewGuid(),
            "Carlos",
            "12345678901",
            DateTime.UtcNow.AddYears(1));

        _driverRepositoryMock
            .Setup(r => r.GetById(existing.Id))
            .Returns(existing);

        var result = _handler.Handle(new GetDriverQuery
        {
            Id = existing.Id
        });

        Assert.NotNull(result);
        Assert.Equal(existing.Id, result!.Id);
        Assert.Equal(existing.Name, result.Name);
        Assert.Equal(existing.LicenseNumber, result.LicenseNumber);
    }
}
using FleetManagement.Domain.Entities;

public class TripDomainTests
{
    [Fact]
    public void Should_Throw_When_VehicleId_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Trip(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2)));
    }

    [Fact]
    public void Should_Throw_When_DriverId_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Trip(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2)));
    }

    [Fact]
    public void Should_Throw_When_StartDate_Is_In_The_Past()
    {
        Assert.Throws<ArgumentException>(() =>
            new Trip(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void Should_Throw_When_EndDate_Is_Before_StartDate()
    {
        Assert.Throws<ArgumentException>(() =>
            new Trip(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void Should_Create_Trip_When_All_Valid()
    {
        var trip = new Trip(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));

        Assert.NotNull(trip);
        Assert.True(trip.StartDate < trip.EndDate);
    }

    [Fact]
    public void Update_Should_Change_Trip_Data()
    {
        var trip = new Trip(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));
        var newVehicleId = Guid.NewGuid();
        var newDriverId = Guid.NewGuid();
        var newStartDate = DateTime.UtcNow.AddDays(3);
        var newEndDate = DateTime.UtcNow.AddDays(4);

        trip.Update(newVehicleId, newDriverId, newStartDate, newEndDate);

        Assert.Equal(newVehicleId, trip.VehicleId);
        Assert.Equal(newDriverId, trip.DriverId);
        Assert.Equal(newStartDate, trip.StartDate);
        Assert.Equal(newEndDate, trip.EndDate);
    }
}

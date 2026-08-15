using FleetManagement.Domain.Entities;
using Xunit;

public class VehicleDomainTests
{
    [Fact]
    public void Should_Throw_When_Id_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Vehicle(Guid.Empty, "ABC1234", "Fiat Uno", 2020));
    }

    [Fact]
    public void Should_Throw_When_LicensePlate_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Vehicle(Guid.NewGuid(), "", "Fiat Uno", 2020));
    }

    [Fact]
    public void Should_Throw_When_LicensePlate_Not_7_Characters()
    {
        Assert.Throws<ArgumentException>(() =>
            new Vehicle(Guid.NewGuid(), "ABC123", "Fiat Uno", 2020)); // 6 chars
    }

    [Fact]
    public void Should_Throw_When_Model_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Vehicle(Guid.NewGuid(), "ABC1234", "", 2020));
    }

    [Fact]
    public void Should_Throw_When_Model_Is_Too_Short()
    {
        Assert.Throws<ArgumentException>(() =>
            new Vehicle(Guid.NewGuid(), "ABC1234", "F", 2020));
    }

    [Fact]
    public void Should_Throw_When_Year_Is_Before_1960()
    {
        Assert.Throws<ArgumentException>(() =>
            new Vehicle(Guid.NewGuid(), "ABC1234", "Fiat Uno", 1950));
    }

    [Fact]
    public void Should_Throw_When_Year_Is_After_Next_Year()
    {
        Assert.Throws<ArgumentException>(() =>
            new Vehicle(Guid.NewGuid(), "ABC1234", "Fiat Uno", DateTime.Now.Year + 2));
    }

    [Fact]
    public void Should_Create_Vehicle_When_All_Valid()
    {
        var vehicle = new Vehicle(Guid.NewGuid(), "ABC1234", "Fiat Uno", 2020);

        Assert.NotNull(vehicle);
        Assert.Equal("ABC1234", vehicle.LicensePlate);
        Assert.Equal("Fiat Uno", vehicle.Model);
        Assert.Equal(2020, vehicle.Year);
    }

    [Fact]
    public void Update_Should_Change_Vehicle_Data()
    {
        var vehicle = new Vehicle(Guid.NewGuid(), "ABC1234", "Fiat Uno", 2020);

        vehicle.Update("XYZ9876", "Toyota Corolla", 2022);

        Assert.Equal("XYZ9876", vehicle.LicensePlate);
        Assert.Equal("Toyota Corolla", vehicle.Model);
        Assert.Equal(2022, vehicle.Year);
    }
}

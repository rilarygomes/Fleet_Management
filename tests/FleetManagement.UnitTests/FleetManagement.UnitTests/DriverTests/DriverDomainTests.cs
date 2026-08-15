using FleetManagement.Domain.Entities;

public class DriverDomainTests
{
    [Fact]
    public void Should_Throw_When_Id_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Driver(Guid.Empty, "Carlos", "ABC12345", DateTime.UtcNow.AddYears(1)));
    }

    [Fact]
    public void Should_Throw_When_Name_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Driver(Guid.NewGuid(), "", "ABC12345", DateTime.UtcNow.AddYears(1)));
    }

    [Fact]
    public void Should_Throw_When_Name_Is_Too_Short()
    {
        Assert.Throws<ArgumentException>(() =>
            new Driver(Guid.NewGuid(), "Jo", "ABC12345", DateTime.UtcNow.AddYears(1)));
    }

    [Fact]
    public void Should_Throw_When_LicenseNumber_Is_Too_Short()
    {
        Assert.Throws<ArgumentException>(() =>
            new Driver(Guid.NewGuid(), "Carlos", "1234", DateTime.UtcNow.AddYears(1)));
    }

    [Fact]
    public void Should_Throw_When_License_Is_Expired()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new Driver(Guid.NewGuid(), "Carlos", "ABC12345", DateTime.UtcNow.AddDays(-1)));
    }

    [Fact]
    public void Should_Create_Driver_When_All_Valid()
    {
        var driver = new Driver(Guid.NewGuid(), "Carlos", "ABC12345", DateTime.UtcNow.AddYears(1));

        Assert.NotNull(driver);
        Assert.Equal("Carlos", driver.Name);
    }

    [Fact]
    public void Should_Throw_When_LicenseNumber_Is_Empty()
    {
        Assert.Throws<ArgumentException>(() =>
            new Driver(Guid.NewGuid(), "Carlos", "", DateTime.UtcNow.AddYears(1)));
    }

}

using FleetManagement.Application.Drivers.Commands.CreateDriver;
using FleetManagement.Application.Drivers.Commands.UpdateDriver;
using FleetManagement.Application.Drivers.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FleetManagement.IntegrationTests.Drivers;

public class DriverControllerIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DriverControllerIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Should_Return_List_Of_Drivers()
    {
        var response = await _client.GetAsync("/api/driver");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var drivers =
            await response.Content.ReadFromJsonAsync<List<DriverDto>>();

        Assert.NotNull(drivers);
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Driver_Does_Not_Exist()
    {
        var response = await _client.GetAsync(
            $"/api/driver/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Should_Return_Created_When_Driver_Is_Valid()
    {
        var command = new CreateDriverCommand
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(1)
        };

        var response = await _client.PostAsJsonAsync(
            "/api/driver",
            command);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var driver =
            await response.Content.ReadFromJsonAsync<DriverDto>();

        Assert.NotNull(driver);
        Assert.NotEqual(Guid.Empty, driver.Id);
        Assert.Equal(command.Name, driver.Name);
        Assert.Equal(command.LicenseNumber, driver.LicenseNumber);
        Assert.Equal(
            command.LicenseExpirationDate.Date,
            driver.LicenseExpirationDate.Date);
    }

    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Driver_Is_Invalid()
    {
        var command = new CreateDriverCommand
        {
            Name = string.Empty,
            LicenseNumber = string.Empty,
            LicenseExpirationDate = DateTime.UtcNow.AddYears(-1)
        };

        var response = await _client.PostAsJsonAsync(
            "/api/driver",
            command);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Update_Should_Return_BadRequest_When_Driver_Not_Found()
    {
        var command = new UpdateDriverCommand
        {
            Name = "Maria",
            LicenseNumber = "98765432101",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(1)
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/driver/{Guid.NewGuid()}",
            command);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Driver_Not_Found()
    {
        var response = await _client.DeleteAsync(
            $"/api/driver/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}
using FleetManagement.Application.Drivers.Commands.CreateDriver;
using FleetManagement.Application.Drivers.DTOs;
using FleetManagement.Application.Trips.Commands.CreateTrip;
using FleetManagement.Application.Trips.Commands.UpdateTrip;
using FleetManagement.Application.Trips.DTOs;
using FleetManagement.Application.Vehicles.Commands.CreateVehicle;
using FleetManagement.Application.Vehicles.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FleetManagement.IntegrationTests.Trips;

public class TripControllerIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TripControllerIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Should_Return_OK()
    {
        var response = await _client.GetAsync("/api/trip");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var trips = await response.Content
            .ReadFromJsonAsync<List<TripDto>>();

        Assert.NotNull(trips);
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Trip_Does_Not_Exist()
    {
        var response = await _client.GetAsync(
            $"/api/trip/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Create_Should_Return_Created_When_Trip_Is_Valid()
    {
        var driverCommand = new CreateDriverCommand
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(1)
        };

        var driverResponse = await _client.PostAsJsonAsync(
            "/api/driver",
            driverCommand);

        Assert.Equal(
            HttpStatusCode.Created,
            driverResponse.StatusCode);

        var driver = await driverResponse.Content
            .ReadFromJsonAsync<DriverDto>();

        Assert.NotNull(driver);

        var vehicleCommand = new CreateVehicleCommand
        {
            LicensePlate = "ABC1234",
            Model = "Fiat Uno",
            Year = 2020
        };

        var vehicleResponse = await _client.PostAsJsonAsync(
            "/api/vehicle",
            vehicleCommand);

        Assert.Equal(
            HttpStatusCode.Created,
            vehicleResponse.StatusCode);

        var vehicle = await vehicleResponse.Content
            .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(vehicle);

        var startDate = DateTime.UtcNow.AddDays(2);
        var endDate = startDate.AddHours(2);

        var tripCommand = new CreateTripCommand
        {
            DriverId = driver.Id,
            VehicleId = vehicle.Id,
            StartDate = startDate,
            EndDate = endDate
        };

        var response = await _client.PostAsJsonAsync(
            "/api/trip",
            tripCommand);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var trip = await response.Content
            .ReadFromJsonAsync<TripDto>();

        Assert.NotNull(trip);
        Assert.NotEqual(Guid.Empty, trip.Id);
        Assert.Equal(driver.Id, trip.DriverId);
        Assert.Equal(vehicle.Id, trip.VehicleId);
        Assert.Equal(startDate, trip.StartDate);
        Assert.Equal(endDate, trip.EndDate);
    }

    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Trip_Is_Invalid()
    {
        var command = new CreateTripCommand
        {
            DriverId = Guid.Empty,
            VehicleId = Guid.Empty,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(-1)
        };

        var response = await _client.PostAsJsonAsync(
            "/api/trip",
            command);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Update_Should_Return_BadRequest_When_Trip_Not_Found()
    {
        var command = new UpdateTripCommand
        {
            DriverId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/trip/{Guid.NewGuid()}",
            command);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Trip_Not_Found()
    {
        var response = await _client.DeleteAsync(
            $"/api/trip/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}
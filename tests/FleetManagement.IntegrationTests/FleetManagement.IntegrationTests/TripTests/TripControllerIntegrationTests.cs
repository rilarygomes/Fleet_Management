using FleetManagement.Api.Common;
using FleetManagement.Application.Drivers.Commands.CreateDriver;
using FleetManagement.Application.Drivers.DTOs;
using FleetManagement.Application.Trips.Commands.CreateTrip;
using FleetManagement.Application.Trips.Commands.UpdateTrip;
using FleetManagement.Application.Trips.DTOs;
using FleetManagement.Application.Vehicles.Commands.CreateVehicle;
using FleetManagement.Application.Vehicles.DTOs;
using System.Net;
using System.Net.Http.Json;

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

        var driverApiResponse = await driverResponse.Content
            .ReadFromJsonAsync<ApiResponse<DriverDto>>();

        Assert.NotNull(driverApiResponse);
        Assert.True(driverApiResponse.Success);
        Assert.NotNull(driverApiResponse.Data);

        var driver = driverApiResponse.Data;

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

        var vehicleApiResponse = await vehicleResponse.Content
            .ReadFromJsonAsync<ApiResponse<VehicleDto>>();

        Assert.NotNull(vehicleApiResponse);
        Assert.True(vehicleApiResponse.Success);
        Assert.NotNull(vehicleApiResponse.Data);

        var vehicle = vehicleApiResponse.Data;

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

        var tripApiResponse = await response.Content
            .ReadFromJsonAsync<ApiResponse<TripDto>>();

        Assert.NotNull(tripApiResponse);
        Assert.True(tripApiResponse.Success);
        Assert.Equal(
            "Trip created successfully.",
            tripApiResponse.Message);

        Assert.NotNull(tripApiResponse.Data);

        var trip = tripApiResponse.Data;

        Assert.NotEqual(Guid.Empty, trip.Id);
        Assert.Equal(driver.Id, trip.DriverId);
        Assert.Equal(vehicle.Id, trip.VehicleId);
        Assert.Equal(startDate, trip.StartDate);
        Assert.Equal(endDate, trip.EndDate);
    }

    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Trip_Is_Invalid()
    {
        var now = DateTime.UtcNow;

        var command = new CreateTripCommand
        {
            DriverId = Guid.Empty,
            VehicleId = Guid.Empty,
            StartDate = now,
            EndDate = now.AddHours(-1)
        };

        var response = await _client.PostAsJsonAsync(
            "/api/trip",
            command);

        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine(content);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        var apiResponse = await response.Content
            .ReadFromJsonAsync<ApiResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.False(string.IsNullOrWhiteSpace(apiResponse.Message));
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

        var apiResponse = await response.Content
            .ReadFromJsonAsync<ApiResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Equal(
            "Trip not found.",
            apiResponse.Message);
    }

    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Trip_Not_Found()
    {
        var response = await _client.DeleteAsync(
            $"/api/trip/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        var apiResponse = await response.Content
            .ReadFromJsonAsync<ApiResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Equal(
            "Trip not found.",
            apiResponse.Message);
    }
}
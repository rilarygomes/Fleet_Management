using FleetManagement.Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

public class TripControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TripControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // --- GETALL ---
    [Fact]
    public async Task GetAll_Should_Return_OK()
    {
        var response = await _client.GetAsync("/api/trip");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // --- GETBYID ---
    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Trip_Not_Exists()
    {
        var response = await _client.GetAsync($"/api/trip/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // --- CREATE ---
    [Fact]
    public async Task Create_Should_Return_Created_When_Trip_Is_Valid()
    {
        // Cria Driver válido
        var driverDto = new CreateDriverDto
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(1)
        };
        var driverResponse = await _client.PostAsJsonAsync("/api/driver", driverDto);
        var driverJson = await driverResponse.Content.ReadFromJsonAsync<JsonElement>();
        var driverId = Guid.Parse(driverJson.GetProperty("data").GetProperty("id").GetString());

        // Cria Vehicle válido
        var vehicleDto = new CreateVehicleDto
        {
            LicensePlate = "ABC1234",
            Model = "Fiat Uno",
            Year = 2020
        };
        var vehicleResponse = await _client.PostAsJsonAsync("/api/vehicle", vehicleDto);
        var vehicleJson = await vehicleResponse.Content.ReadFromJsonAsync<JsonElement>();
        var vehicleId = Guid.Parse(vehicleJson.GetProperty("data").GetProperty("id").GetString());

        // Usa datas futuras para evitar erro
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddHours(2);

        var tripDto = new CreateTripDto
        {
            DriverId = driverId,
            VehicleId = vehicleId,
            StartDate = startDate,
            EndDate = endDate
        };

        var response = await _client.PostAsJsonAsync("/api/trip", tripDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var tripJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Trip created successfully.", tripJson.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Trip_Invalid()
    {
        var dto = new CreateTripDto
        {
            DriverId = Guid.Empty,
            VehicleId = Guid.Empty,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(-1)
        };

        var response = await _client.PostAsJsonAsync("/api/trip", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- UPDATE ---
    [Fact]
    public async Task Update_Should_Return_BadRequest_When_Trip_Not_Found()
    {
        var dto = new UpdateTripDto
        {
            DriverId = Guid.NewGuid(),
            VehicleId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(3)
        };

        var response = await _client.PutAsJsonAsync($"/api/trip/{Guid.NewGuid()}", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- DELETE ---
    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Trip_Not_Found()
    {
        var response = await _client.DeleteAsync($"/api/trip/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

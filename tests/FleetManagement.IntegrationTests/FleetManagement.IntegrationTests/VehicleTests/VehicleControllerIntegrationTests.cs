using FleetManagement.Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

public class VehicleControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public VehicleControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Should_Return_OK()
    {
        var response = await _client.GetAsync("/api/vehicle");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Vehicle_Not_Exists()
    {
        var response = await _client.GetAsync($"/api/vehicle/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_Should_Return_Created_When_Vehicle_Is_Valid()
    {
        var dto = new CreateVehicleDto
        {
            LicensePlate = "ABC1234",
            Model = "Fiat Uno",
            Year = 2020
        };

        var response = await _client.PostAsJsonAsync("/api/vehicle", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Vehicle created successfully.", json.GetProperty("message").GetString());
    }


    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Vehicle_Invalid()
    {
        var dto = new CreateVehicleDto
        {
            LicensePlate = "",
            Model = "",
            Year = 0
        };

        var response = await _client.PostAsJsonAsync("/api/vehicle", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_Should_Return_Ok_When_Vehicle_Exists()
    {
        var createDto = new CreateVehicleDto
        {
            LicensePlate = "XYZ9876",
            Model = "VW Gol",
            Year = 2022
        };

        var createResponse = await _client.PostAsJsonAsync("/api/vehicle", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdJson = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = Guid.Parse(createdJson.GetProperty("data").GetProperty("id").GetString());

        var updateDto = new UpdateVehicleDto
        {
            LicensePlate = "XYZ9876",
            Model = "VW Gol G4",
            Year = 2023
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/vehicle/{id}", updateDto);

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
    }

    [Fact]
    public async Task Update_Should_Return_BadRequest_When_Vehicle_Not_Found()
    {
        var dto = new UpdateVehicleDto
        {
            LicensePlate = "XYZ9876",
            Model = "VW Gol",
            Year = 2022
        };

        var response = await _client.PutAsJsonAsync($"/api/vehicle/{Guid.NewGuid()}", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_Ok_When_Vehicle_Exists()
    {
        var createDto = new CreateVehicleDto
        {
            LicensePlate = "DEL1234",
            Model = "Ford Ka",
            Year = 2019
        };

        var createResponse = await _client.PostAsJsonAsync("/api/vehicle", createDto);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdJson = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = Guid.Parse(createdJson.GetProperty("data").GetProperty("id").GetString());

        var deleteResponse = await _client.DeleteAsync($"/api/vehicle/{id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Vehicle_Not_Found()
    {
        var response = await _client.DeleteAsync($"/api/vehicle/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

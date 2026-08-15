using FleetManagement.Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

public class DriverControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DriverControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // --- GETALL ---
    [Fact]
    public async Task GetAll_Should_Return_List_Of_Drivers()
    {
        var response = await _client.GetAsync("/api/driver");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // --- GETBYID ---
    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Driver_Not_Exists()
    {
        var response = await _client.GetAsync($"/api/driver/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // --- CREATE ---
    [Fact]
    public async Task Create_Should_Return_Created_When_Driver_Is_Valid()
    {
        var dto = new CreateDriverDto
        {
            Name = "Carlos",
            LicenseNumber = "12345678901",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(1)
        };

        var response = await _client.PostAsJsonAsync("/api/driver", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Driver created successfully.", json.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Driver_Invalid()
    {
        var dto = new CreateDriverDto
        {
            Name = "",
            LicenseNumber = "",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(-1)
        };

        var response = await _client.PostAsJsonAsync("/api/driver", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- UPDATE ---
    [Fact]
    public async Task Update_Should_Return_BadRequest_When_Driver_Not_Found()
    {
        var dto = new UpdateDriverDto
        {
            Name = "Maria",
            LicenseNumber = "98765432101",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(1)
        };

        var response = await _client.PutAsJsonAsync($"/api/driver/{Guid.NewGuid()}", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // --- DELETE ---
    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Driver_Not_Found()
    {
        var response = await _client.DeleteAsync($"/api/driver/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

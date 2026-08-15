using FleetManagement.Application.Vehicles.Commands.CreateVehicle;
using FleetManagement.Application.Vehicles.Commands.UpdateVehicle;
using FleetManagement.Application.Vehicles.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FleetManagement.IntegrationTests.Vehicles;

public class VehicleControllerIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public VehicleControllerIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Should_Return_OK()
    {
        var response = await _client.GetAsync("/api/vehicle");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var vehicles = await response.Content
            .ReadFromJsonAsync<List<VehicleDto>>();

        Assert.NotNull(vehicles);
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Vehicle_Does_Not_Exist()
    {
        var response = await _client.GetAsync(
            $"/api/vehicle/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Create_Should_Return_Created_When_Vehicle_Is_Valid()
    {
        var command = new CreateVehicleCommand
        {
            LicensePlate = "ABC1234",
            Model = "Fiat Uno",
            Year = 2020
        };

        var response = await _client.PostAsJsonAsync(
            "/api/vehicle",
            command);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var vehicle = await response.Content
            .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(vehicle);
        Assert.NotEqual(Guid.Empty, vehicle.Id);
        Assert.Equal(command.LicensePlate, vehicle.LicensePlate);
        Assert.Equal(command.Model, vehicle.Model);
        Assert.Equal(command.Year, vehicle.Year);
    }

    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Vehicle_Is_Invalid()
    {
        var command = new CreateVehicleCommand
        {
            LicensePlate = string.Empty,
            Model = string.Empty,
            Year = 0
        };

        var response = await _client.PostAsJsonAsync(
            "/api/vehicle",
            command);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Update_Should_Return_OK_When_Vehicle_Exists()
    {
        var createCommand = new CreateVehicleCommand
        {
            LicensePlate = "XYZ9876",
            Model = "VW Gol",
            Year = 2022
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/vehicle",
            createCommand);

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var createdVehicle = await createResponse.Content
            .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(createdVehicle);

        var updateCommand = new UpdateVehicleCommand
        {
            LicensePlate = "XYZ9876",
            Model = "VW Gol G4",
            Year = 2023
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/vehicle/{createdVehicle.Id}",
            updateCommand);

        Assert.Equal(
            HttpStatusCode.OK,
            updateResponse.StatusCode);

        var updatedVehicle = await updateResponse.Content
            .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(updatedVehicle);
        Assert.Equal(updateCommand.LicensePlate, updatedVehicle.LicensePlate);
        Assert.Equal(updateCommand.Model, updatedVehicle.Model);
        Assert.Equal(updateCommand.Year, updatedVehicle.Year);
    }

    [Fact]
    public async Task Update_Should_Return_BadRequest_When_Vehicle_Not_Found()
    {
        var command = new UpdateVehicleCommand
        {
            LicensePlate = "XYZ9876",
            Model = "VW Gol",
            Year = 2022
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/vehicle/{Guid.NewGuid()}",
            command);

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_NoContent_When_Vehicle_Exists()
    {
        var createCommand = new CreateVehicleCommand
        {
            LicensePlate = "DEL1234",
            Model = "Ford Ka",
            Year = 2019
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/vehicle",
            createCommand);

        Assert.Equal(
            HttpStatusCode.Created,
            createResponse.StatusCode);

        var createdVehicle = await createResponse.Content
            .ReadFromJsonAsync<VehicleDto>();

        Assert.NotNull(createdVehicle);

        var deleteResponse = await _client.DeleteAsync(
            $"/api/vehicle/{createdVehicle.Id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Vehicle_Not_Found()
    {
        var response = await _client.DeleteAsync(
            $"/api/vehicle/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }
}
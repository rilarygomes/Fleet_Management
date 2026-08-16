using FleetManagement.Api.Common;
using FleetManagement.Application.Vehicles.Commands.CreateVehicle;
using FleetManagement.Application.Vehicles.Commands.UpdateVehicle;
using FleetManagement.Application.Vehicles.DTOs;
using System.Net;
using System.Net.Http.Json;

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

        var apiResponse = await response.Content
            .ReadFromJsonAsync<ApiResponse<VehicleDto>>();

        Assert.NotNull(apiResponse);
        Assert.True(apiResponse.Success);
        Assert.Equal(
            "Vehicle created successfully.",
            apiResponse.Message);

        Assert.NotNull(apiResponse.Data);

        var vehicle = apiResponse.Data;

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

        var apiResponse = await response.Content
            .ReadFromJsonAsync<ApiResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.False(
            string.IsNullOrWhiteSpace(apiResponse.Message));
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

        var createApiResponse = await createResponse.Content
            .ReadFromJsonAsync<ApiResponse<VehicleDto>>();

        Assert.NotNull(createApiResponse);
        Assert.True(createApiResponse.Success);
        Assert.NotNull(createApiResponse.Data);

        var createdVehicle = createApiResponse.Data;

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

        var updateApiResponse = await updateResponse.Content
            .ReadFromJsonAsync<ApiResponse<VehicleDto>>();

        Assert.NotNull(updateApiResponse);
        Assert.True(updateApiResponse.Success);
        Assert.Equal(
            "Vehicle updated successfully.",
            updateApiResponse.Message);

        Assert.NotNull(updateApiResponse.Data);

        var updatedVehicle = updateApiResponse.Data;

        Assert.Equal(
            updateCommand.LicensePlate,
            updatedVehicle.LicensePlate);

        Assert.Equal(
            updateCommand.Model,
            updatedVehicle.Model);

        Assert.Equal(
            updateCommand.Year,
            updatedVehicle.Year);
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

        var apiResponse = await response.Content
            .ReadFromJsonAsync<ApiResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Equal(
            "Vehicle not found.",
            apiResponse.Message);
    }

    [Fact]
    public async Task Delete_Should_Return_OK_When_Vehicle_Exists()
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

        var createApiResponse = await createResponse.Content
            .ReadFromJsonAsync<ApiResponse<VehicleDto>>();

        Assert.NotNull(createApiResponse);
        Assert.True(createApiResponse.Success);
        Assert.NotNull(createApiResponse.Data);

        var createdVehicle = createApiResponse.Data;

        var deleteResponse = await _client.DeleteAsync(
            $"/api/vehicle/{createdVehicle.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            deleteResponse.StatusCode);

        var deleteApiResponse = await deleteResponse.Content
            .ReadFromJsonAsync<ApiResponse>();

        Assert.NotNull(deleteApiResponse);
        Assert.True(deleteApiResponse.Success);
        Assert.Equal(
            "Vehicle deleted successfully.",
            deleteApiResponse.Message);
    }

    [Fact]
    public async Task Delete_Should_Return_BadRequest_When_Vehicle_Not_Found()
    {
        var response = await _client.DeleteAsync(
            $"/api/vehicle/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);

        var apiResponse = await response.Content
            .ReadFromJsonAsync<ApiResponse>();

        Assert.NotNull(apiResponse);
        Assert.False(apiResponse.Success);
        Assert.Equal(
            "Vehicle not found.",
            apiResponse.Message);
    }
}
using FleetManagement.Api.Common;
using FleetManagement.Api.Swagger;
using FleetManagement.Application.Vehicles.Commands.CreateVehicle;
using FleetManagement.Application.Vehicles.Commands.DeleteVehicle;
using FleetManagement.Application.Vehicles.Commands.UpdateVehicle;
using FleetManagement.Application.Vehicles.DTOs;
using FleetManagement.Application.Vehicles.GetVehicle;
using FleetManagement.Application.Vehicles.GetVehicles;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly CreateVehicleCommandHandler _createVehicleHandler;
    private readonly UpdateVehicleCommandHandler _updateVehicleHandler;
    private readonly DeleteVehicleCommandHandler _deleteVehicleHandler;
    private readonly GetVehicleQueryHandler _getVehicleHandler;
    private readonly GetVehiclesQueryHandler _getVehiclesHandler;

    public VehicleController(
        CreateVehicleCommandHandler createVehicleHandler,
        UpdateVehicleCommandHandler updateVehicleHandler,
        DeleteVehicleCommandHandler deleteVehicleHandler,
        GetVehicleQueryHandler getVehicleHandler,
        GetVehiclesQueryHandler getVehiclesHandler)
    {
        _createVehicleHandler = createVehicleHandler;
        _updateVehicleHandler = updateVehicleHandler;
        _deleteVehicleHandler = deleteVehicleHandler;
        _getVehicleHandler = getVehicleHandler;
        _getVehiclesHandler = getVehiclesHandler;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<VehicleDto>),
        StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] string? licensePlate)
    {
        var query = new GetVehiclesQuery
        {
            LicensePlate = licensePlate
        };

        var result = _getVehiclesHandler.Handle(query);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(VehicleDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(
        StatusCodes.Status200OK,
        typeof(VehicleDtoExample))]
    public IActionResult GetById(Guid id)
    {
        var query = new GetVehicleQuery
        {
            Id = id
        };

        var result = _getVehicleHandler.Handle(query);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<VehicleDto>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiResponse),
        StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(
        typeof(CreateVehicleCommand),
        typeof(CreateVehicleCommandExample))]
    public IActionResult Create(
        [FromBody] CreateVehicleCommand command)
    {
        var result = _createVehicleHandler.Handle(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = result.Error ?? "Unable to create vehicle."
            });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            new ApiResponse<VehicleDto>
            {
                Success = true,
                Message = "Vehicle created successfully.",
                Data = result.Value
            });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<VehicleDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse),
        StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(
        typeof(UpdateVehicleCommand),
        typeof(UpdateVehicleCommandExample))]
    public IActionResult Update(
        Guid id,
        [FromBody] UpdateVehicleCommand command)
    {
        var result = _updateVehicleHandler.Handle(id, command);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = result.Error ?? "Unable to update vehicle."
            });
        }

        return Ok(new ApiResponse<VehicleDto>
        {
            Success = true,
            Message = "Vehicle updated successfully.",
            Data = result.Value
        });
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse),
        StatusCodes.Status400BadRequest)]
    public IActionResult Delete(Guid id)
    {
        var command = new DeleteVehicleCommand
        {
            Id = id
        };

        var result = _deleteVehicleHandler.Handle(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = result.Error ?? "Unable to delete vehicle."
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Vehicle deleted successfully."
        });
    }
}
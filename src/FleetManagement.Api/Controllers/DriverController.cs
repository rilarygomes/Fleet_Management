using FleetManagement.Api.Common;
using FleetManagement.Api.Swagger;
using FleetManagement.Application.Drivers.Commands.CreateDriver;
using FleetManagement.Application.Drivers.Commands.DeleteDriver;
using FleetManagement.Application.Drivers.Commands.UpdateDriver;
using FleetManagement.Application.Drivers.DTOs;
using FleetManagement.Application.Drivers.GetDriver;
using FleetManagement.Application.Drivers.GetDrivers;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriverController : ControllerBase
{
    private readonly CreateDriverCommandHandler _createDriverHandler;
    private readonly UpdateDriverCommandHandler _updateDriverHandler;
    private readonly DeleteDriverCommandHandler _deleteDriverHandler;
    private readonly GetDriverQueryHandler _getDriverHandler;
    private readonly GetDriversQueryHandler _getDriversHandler;

    public DriverController(
        CreateDriverCommandHandler createDriverHandler,
        UpdateDriverCommandHandler updateDriverHandler,
        DeleteDriverCommandHandler deleteDriverHandler,
        GetDriverQueryHandler getDriverHandler,
        GetDriversQueryHandler getDriversHandler)
    {
        _createDriverHandler = createDriverHandler;
        _updateDriverHandler = updateDriverHandler;
        _deleteDriverHandler = deleteDriverHandler;
        _getDriverHandler = getDriverHandler;
        _getDriversHandler = getDriversHandler;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IEnumerable<DriverDto>),
        StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] string? name)
    {
        var query = new GetDriversQuery
        {
            Name = name
        };

        var result = _getDriversHandler.Handle(query);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(
        typeof(DriverDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(
        StatusCodes.Status200OK,
        typeof(DriverDtoExample))]
    public IActionResult GetById(Guid id)
    {
        var query = new GetDriverQuery
        {
            Id = id
        };

        var result = _getDriverHandler.Handle(query);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<DriverDto>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ApiResponse),
        StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(
        typeof(CreateDriverCommand),
        typeof(CreateDriverCommandExample))]
    public IActionResult Create(
        [FromBody] CreateDriverCommand command)
    {
        var result = _createDriverHandler.Handle(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = result.Error ?? "Unable to create driver."
            });
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            new ApiResponse<DriverDto>
            {
                Success = true,
                Message = "Driver created successfully.",
                Data = result.Value
            });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(
        typeof(ApiResponse<DriverDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ApiResponse),
        StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(
        typeof(UpdateDriverCommand),
        typeof(UpdateDriverCommandExample))]
    public IActionResult Update(
        Guid id,
        [FromBody] UpdateDriverCommand command)
    {
        var result = _updateDriverHandler.Handle(id, command);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = result.Error ?? "Unable to update driver."
            });
        }

        return Ok(new ApiResponse<DriverDto>
        {
            Success = true,
            Message = "Driver updated successfully.",
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
        var command = new DeleteDriverCommand
        {
            Id = id
        };

        var result = _deleteDriverHandler.Handle(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = result.Error ?? "Unable to delete driver."
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Driver deleted successfully."
        });
    }
}
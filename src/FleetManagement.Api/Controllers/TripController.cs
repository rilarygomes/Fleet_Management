using FleetManagement.Api.Swagger;
using FleetManagement.Application.Trips.Commands.CreateTrip;
using FleetManagement.Application.Trips.Commands.DeleteTrip;
using FleetManagement.Application.Trips.Commands.UpdateTrip;
using FleetManagement.Application.Trips.DTOs;
using FleetManagement.Application.Trips.GetTrip;
using FleetManagement.Application.Trips.GetTrips;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripController : ControllerBase
{
    private readonly CreateTripCommandHandler _createTripHandler;
    private readonly UpdateTripCommandHandler _updateTripHandler;
    private readonly DeleteTripCommandHandler _deleteTripHandler;
    private readonly GetTripQueryHandler _getTripHandler;
    private readonly GetTripsQueryHandler _getTripsHandler;

    public TripController(
        CreateTripCommandHandler createTripHandler,
        UpdateTripCommandHandler updateTripHandler,
        DeleteTripCommandHandler deleteTripHandler,
        GetTripQueryHandler getTripHandler,
        GetTripsQueryHandler getTripsHandler)
    {
        _createTripHandler = createTripHandler;
        _updateTripHandler = updateTripHandler;
        _deleteTripHandler = deleteTripHandler;
        _getTripHandler = getTripHandler;
        _getTripsHandler = getTripsHandler;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TripDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] Guid? driverId,
        [FromQuery] Guid? vehicleId)
    {
        var query = new GetTripsQuery
        {
            StartDate = startDate,
            EndDate = endDate,
            DriverId = driverId,
            VehicleId = vehicleId
        };

        var result = _getTripsHandler.Handle(query);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(
        StatusCodes.Status200OK,
        typeof(TripDtoExample))]
    public IActionResult GetById(Guid id)
    {
        var query = new GetTripQuery
        {
            Id = id
        };

        var result = _getTripHandler.Handle(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(
        typeof(CreateTripCommand),
        typeof(CreateTripCommandExample))]
    [SwaggerResponseExample(
        StatusCodes.Status201Created,
        typeof(TripDtoExample))]
    public IActionResult Create([FromBody] CreateTripCommand command)
    {
        var result = _createTripHandler.Handle(command);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(
        typeof(UpdateTripCommand),
        typeof(UpdateTripCommandExample))]
    [SwaggerResponseExample(
        StatusCodes.Status200OK,
        typeof(TripDtoExample))]
    public IActionResult Update(
        Guid id,
        [FromBody] UpdateTripCommand command)
    {
        command.Id = id;

        var result = _updateTripHandler.Handle(command);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Delete(Guid id)
    {
        var command = new DeleteTripCommand
        {
            Id = id
        };

        var result = _deleteTripHandler.Handle(command);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return NoContent();
    }
}
using FleetManagement.Application.DTOs;
using FleetManagement.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class TripController : ControllerBase
{
    private readonly TripService _tripService;
    private readonly IValidator<CreateTripDto> _createTripValidator;
    private readonly IValidator<UpdateTripDto> _updateTripValidator;
    private readonly ILogger<TripController> _logger;

    public TripController(
        TripService tripService,
        IValidator<CreateTripDto> createTripValidator,
        IValidator<UpdateTripDto> updateTripValidator,
        ILogger<TripController> logger)
    {
        _tripService = tripService;
        _createTripValidator = createTripValidator;
        _updateTripValidator = updateTripValidator;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] Guid? driverId,
        [FromQuery] Guid? vehicleId)
    {
        _logger.LogInformation("Fetching all trips with filters: startDate={StartDate}, endDate={EndDate}, driverId={DriverId}, vehicleId={VehicleId}",
            startDate, endDate, driverId, vehicleId);

        var trips = _tripService.GetAll();

        if (startDate.HasValue)
            trips = trips.Where(t => t.StartDate >= startDate.Value);

        if (endDate.HasValue)
            trips = trips.Where(t => t.EndDate <= endDate.Value);

        if (driverId.HasValue)
            trips = trips.Where(t => t.DriverId == driverId.Value);

        if (vehicleId.HasValue)
            trips = trips.Where(t => t.VehicleId == vehicleId.Value);

        return Ok(trips);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Fetching trip by Id {TripId}", id);
        var t = _tripService.GetById(id);
        if (t == null)
        {
            _logger.LogWarning("Trip {TripId} not found", id);
            return NotFound();
        }

        return Ok(t);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateTripDto dto)
    {
        _logger.LogInformation("Creating new trip with Vehicle {VehicleId} and Driver {DriverId}", dto.VehicleId, dto.DriverId);
        var validation = _createTripValidator.Validate(dto);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Trip creation validation failed: {Errors}", validation.Errors);
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }

        var result = _tripService.Add(dto);

        if (!result.IsSuccess)
        {
            _logger.LogError("Trip creation failed: {Error}", result.Error);
            return BadRequest(new { Message = result.Error });
        }

        _logger.LogInformation("Trip {TripId} created successfully", result.Value.Id);
        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, new
        {
            Message = "Trip created successfully.",
            Data = result.Value
        });
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] UpdateTripDto dto)
    {
        _logger.LogInformation("Updating trip {TripId}", id);
        var validation = _updateTripValidator.Validate(dto);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Trip update validation failed for {TripId}", id);
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }

        var result = _tripService.Update(id, dto);

        if (!result.IsSuccess)
        {
            _logger.LogError("Trip update failed for {TripId}: {Error}", id, result.Error);
            return BadRequest(new { Message = result.Error });
        }

        _logger.LogInformation("Trip {TripId} updated successfully", id);
        return Ok(new
        {
            Message = "Trip updated successfully.",
            Data = result.Value
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Deleting trip {TripId}", id);
        var result = _tripService.Remove(id);

        if (!result.IsSuccess)
        {
            _logger.LogError("Trip deletion failed for {TripId}: {Error}", id, result.Error);
            return BadRequest(new { Message = result.Error });
        }

        _logger.LogInformation("Trip {TripId} deleted successfully", id);
        return Ok(new { Message = "Trip deleted successfully." });
    }
}

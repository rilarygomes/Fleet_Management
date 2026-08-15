using FleetManagement.Application.DTOs;
using FleetManagement.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TripController : ControllerBase
{
    private readonly TripService _tripService;
    private readonly IValidator<CreateTripDto> _createTripValidator;
    private readonly IValidator<UpdateTripDto> _updateTripValidator;

    public TripController(
        TripService tripService,
        IValidator<CreateTripDto> createTripValidator,
        IValidator<UpdateTripDto> updateTripValidator)
    {
        _tripService = tripService;
        _createTripValidator = createTripValidator;
        _updateTripValidator = updateTripValidator;
    }

    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] Guid? driverId,
        [FromQuery] Guid? vehicleId)
    {
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
        var t = _tripService.GetById(id);
        if (t == null) return NotFound();

        return Ok(t);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateTripDto dto)
    {
        var validation = _createTripValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });

        var result = _tripService.Add(dto);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, new
        {
            Message = "Trip created successfully.",
            Data = result.Value
        });
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] UpdateTripDto dto)
    {
        var validation = _updateTripValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });

        var result = _tripService.Update(id, dto);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return Ok(new
        {
            Message = "Trip updated successfully.",
            Data = result.Value
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var result = _tripService.Remove(id);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return Ok(new { Message = "Trip deleted successfully." });
    }
}

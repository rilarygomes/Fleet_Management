using FleetManagement.Application.DTOs;
using FleetManagement.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly VehicleService _vehicleService;
    private readonly IValidator<CreateVehicleDto> _createVehicleValidator;
    private readonly IValidator<UpdateVehicleDto> _updateVehicleValidator;

    public VehicleController(
        VehicleService vehicleService,
        IValidator<CreateVehicleDto> createVehicleValidator,
        IValidator<UpdateVehicleDto> updateVehicleValidator)
    {
        _vehicleService = vehicleService;
        _createVehicleValidator = createVehicleValidator;
        _updateVehicleValidator = updateVehicleValidator;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? licensePlate)
    {
        var vehicles = _vehicleService.GetAll();

        if (!string.IsNullOrEmpty(licensePlate))
            vehicles = vehicles.Where(v =>
                v.LicensePlate.Contains(licensePlate, StringComparison.OrdinalIgnoreCase));

        return Ok(vehicles);
    }


    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var v = _vehicleService.GetById(id);
        if (v == null) return NotFound();

        return Ok(v);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateVehicleDto dto)
    {
        var validation = _createVehicleValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });

        var result = _vehicleService.Add(dto);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, new
        {
            Message = "Vehicle created successfully.",
            Data = result.Value
        });
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] UpdateVehicleDto dto)
    {
        var validation = _updateVehicleValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });

        var result = _vehicleService.Update(id, dto);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return Ok(new
        {
            Message = "Vehicle updated successfully.",
            Data = result.Value
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var result = _vehicleService.Remove(id);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return Ok(new { Message = "Vehicle deleted successfully." });
    }
}

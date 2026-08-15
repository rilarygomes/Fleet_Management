using FleetManagement.Application.DTOs;
using FleetManagement.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly VehicleService _vehicleService;
    private readonly IValidator<CreateVehicleDto> _createVehicleValidator;
    private readonly IValidator<UpdateVehicleDto> _updateVehicleValidator;
    private readonly ILogger<VehicleController> _logger;

    public VehicleController(
        VehicleService vehicleService,
        IValidator<CreateVehicleDto> createVehicleValidator,
        IValidator<UpdateVehicleDto> updateVehicleValidator,
        ILogger<VehicleController> logger)
    {
        _vehicleService = vehicleService;
        _createVehicleValidator = createVehicleValidator;
        _updateVehicleValidator = updateVehicleValidator;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? licensePlate)
    {
        _logger.LogInformation("Fetching all vehicles with filter licensePlate={LicensePlate}", licensePlate);
        var vehicles = _vehicleService.GetAll();

        if (!string.IsNullOrEmpty(licensePlate))
            vehicles = vehicles.Where(v =>
                v.LicensePlate.Contains(licensePlate, StringComparison.OrdinalIgnoreCase));

        return Ok(vehicles);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Fetching vehicle by Id {VehicleId}", id);
        var v = _vehicleService.GetById(id);
        if (v == null)
        {
            _logger.LogWarning("Vehicle {VehicleId} not found", id);
            return NotFound();
        }

        return Ok(v);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateVehicleDto dto)
    {
        _logger.LogInformation("Creating new vehicle with LicensePlate {LicensePlate}", dto.LicensePlate);
        var validation = _createVehicleValidator.Validate(dto);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Vehicle creation validation failed: {Errors}", validation.Errors);
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }

        var result = _vehicleService.Add(dto);

        if (!result.IsSuccess)
        {
            _logger.LogError("Vehicle creation failed: {Error}", result.Error);
            return BadRequest(new { Message = result.Error });
        }

        _logger.LogInformation("Vehicle {VehicleId} created successfully", result.Value.Id);
        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, new
        {
            Message = "Vehicle created successfully.",
            Data = result.Value
        });
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] UpdateVehicleDto dto)
    {
        _logger.LogInformation("Updating vehicle {VehicleId}", id);
        var validation = _updateVehicleValidator.Validate(dto);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Vehicle update validation failed for {VehicleId}", id);
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }

        var result = _vehicleService.Update(id, dto);

        if (!result.IsSuccess)
        {
            _logger.LogError("Vehicle update failed for {VehicleId}: {Error}", id, result.Error);
            return BadRequest(new { Message = result.Error });
        }

        _logger.LogInformation("Vehicle {VehicleId} updated successfully", id);
        return Ok(new
        {
            Message = "Vehicle updated successfully.",
            Data = result.Value
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Deleting vehicle {VehicleId}", id);
        var result = _vehicleService.Remove(id);

        if (!result.IsSuccess)
        {
            _logger.LogError("Vehicle deletion failed for {VehicleId}: {Error}", id, result.Error);
            return BadRequest(new { Message = result.Error });
        }

        _logger.LogInformation("Vehicle {VehicleId} deleted successfully", id);
        return Ok(new { Message = "Vehicle deleted successfully." });
    }
}

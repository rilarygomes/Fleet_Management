using FleetManagement.Application.DTOs;
using FleetManagement.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class DriverController : ControllerBase
{
    private readonly DriverService _driverService;
    private readonly IValidator<CreateDriverDto> _createDriverValidator;
    private readonly IValidator<UpdateDriverDto> _updateDriverValidator;
    private readonly ILogger<DriverController> _logger;

    public DriverController(
        DriverService driverService,
        IValidator<CreateDriverDto> createDriverValidator,
        IValidator<UpdateDriverDto> updateDriverValidator,
        ILogger<DriverController> logger)
    {
        _driverService = driverService;
        _createDriverValidator = createDriverValidator;
        _updateDriverValidator = updateDriverValidator;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? name)
    {
        _logger.LogInformation("Fetching all drivers with filter name={Name}", name);
        var drivers = _driverService.GetAll();

        if (!string.IsNullOrEmpty(name))
            drivers = drivers.Where(d => d.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        return Ok(drivers);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        _logger.LogInformation("Fetching driver by Id {DriverId}", id);
        var d = _driverService.GetById(id);
        if (d == null)
        {
            _logger.LogWarning("Driver {DriverId} not found", id);
            return NotFound();
        }

        return Ok(d);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateDriverDto dto)
    {
        _logger.LogInformation("Creating new driver with LicenseNumber {LicenseNumber}", dto.LicenseNumber);
        var validation = _createDriverValidator.Validate(dto);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Driver creation validation failed: {Errors}", validation.Errors);
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }

        var result = _driverService.Add(dto);

        if (!result.IsSuccess)
        {
            _logger.LogError("Driver creation failed: {Error}", result.Error);
            return BadRequest(new { Message = result.Error });
        }

        _logger.LogInformation("Driver {DriverId} created successfully", result.Value.Id);
        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, new
        {
            Message = "Driver created successfully.",
            Data = result.Value
        });
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] UpdateDriverDto dto)
    {
        _logger.LogInformation("Updating driver {DriverId}", id);
        var validation = _updateDriverValidator.Validate(dto);
        if (!validation.IsValid)
        {
            _logger.LogWarning("Driver update validation failed for {DriverId}", id);
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
        }

        var result = _driverService.Update(id, dto);

        if (!result.IsSuccess)
        {
            _logger.LogError("Driver update failed for {DriverId}: {Error}", id, result.Error);
            return BadRequest(new { Message = result.Error });
        }

        _logger.LogInformation("Driver {DriverId} updated successfully", id);
        return Ok(new
        {
            Message = "Driver updated successfully.",
            Data = result.Value
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _logger.LogInformation("Deleting driver {DriverId}", id);
        var result = _driverService.Remove(id);

        if (!result.IsSuccess)
        {
            _logger.LogError("Driver deletion failed for {DriverId}: {Error}", id, result.Error);
            return BadRequest(new { Message = result.Error });
        }

        _logger.LogInformation("Driver {DriverId} deleted successfully", id);
        return Ok(new { Message = "Driver deleted successfully." });
    }
}

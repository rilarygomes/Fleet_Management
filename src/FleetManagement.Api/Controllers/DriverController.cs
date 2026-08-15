using FleetManagement.Application.DTOs;
using FleetManagement.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DriverController : ControllerBase
{
    private readonly DriverService _driverService;
    private readonly IValidator<CreateDriverDto> _createDriverValidator;
    private readonly IValidator<UpdateDriverDto> _updateDriverValidator;

    public DriverController(
        DriverService driverService,
        IValidator<CreateDriverDto> createDriverValidator,
        IValidator<UpdateDriverDto> updateDriverValidator)
    {
        _driverService = driverService;
        _createDriverValidator = createDriverValidator;
        _updateDriverValidator = updateDriverValidator;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? name)
    {
        var drivers = _driverService.GetAll();

        if (!string.IsNullOrEmpty(name))
            drivers = drivers.Where(d => d.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

        return Ok(drivers);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var d = _driverService.GetById(id);
        if (d == null) return NotFound();

        return Ok(d);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateDriverDto dto)
    {
        var validation = _createDriverValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });

        var result = _driverService.Add(dto);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, new
        {
            Message = "Driver created successfully.",
            Data = result.Value
        });
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, [FromBody] UpdateDriverDto dto)
    {
        var validation = _updateDriverValidator.Validate(dto);
        if (!validation.IsValid)
            return BadRequest(new { Errors = validation.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });

        var result = _driverService.Update(id, dto);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return Ok(new
        {
            Message = "Driver updated successfully.",
            Data = result.Value
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var result = _driverService.Remove(id);

        if (!result.IsSuccess)
            return BadRequest(new { Message = result.Error });

        return Ok(new { Message = "Driver deleted successfully." });
    }
}

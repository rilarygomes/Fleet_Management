using FleetManagement.Application.DTOs;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FleetManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly IDriverRepository _repository;
        private readonly FleetManagementDbContext _context;

        public DriverController(IDriverRepository repository, FleetManagementDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// Retrieves all drivers with an optional filter by name.
        /// </summary>
        /// <param name="name">Optional filter by driver name.</param>
        /// <returns>Returns a list of drivers.</returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? name)
        {
            var drivers = _repository.GetAll();

            if (!string.IsNullOrEmpty(name))
                drivers = drivers.Where(d => d.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            return Ok(drivers.Select(d => new DriverDto
            {
                Id = d.Id,
                Name = d.Name,
                LicenseNumber = d.LicenseNumber,
                LicenseExpirationDate = d.LicenseExpirationDate
            }));
        }

        /// <summary>
        /// Retrieves a driver by its unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the driver.</param>
        /// <returns>Returns the driver information if found.</returns>
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var d = _repository.GetById(id);
            if (d == null) return NotFound();

            return Ok(new DriverDto { Id = d.Id, Name = d.Name, LicenseNumber = d.LicenseNumber, LicenseExpirationDate = d.LicenseExpirationDate });
        }

        /// <summary>
        /// Creates a new driver.
        /// </summary>
        /// <param name="dto">Driver data transfer object containing name, license number, and expiration date.</param>
        /// <returns>Returns the created driver information.</returns>
        [HttpPost]
        public IActionResult Create([FromBody] DriverDto dto)
        {
            var driver = new Driver(Guid.NewGuid(), dto.Name, dto.LicenseNumber, dto.LicenseExpirationDate);
            _repository.Add(driver);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = driver.Id }, dto);
        }

        /// <summary>
        /// Updates an existing driver.
        /// </summary>
        /// <param name="id">Unique identifier of the driver to update.</param>
        /// <param name="dto">Driver data transfer object containing updated values.</param>
        /// <returns>Returns the updated driver information.</returns>
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] UpdateDriverDto dto)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return NotFound("Driver not found.");

            if (dto.LicenseExpirationDate < DateTime.Now)
                return BadRequest("License expiration date cannot be in the past.");

            existing.Update(dto.Name, dto.LicenseNumber, dto.LicenseExpirationDate);

            _repository.Update(existing);
            _context.SaveChanges();

            return Ok(new DriverDto
            {
                Id = existing.Id,
                Name = existing.Name,
                LicenseNumber = existing.LicenseNumber,
                LicenseExpirationDate = existing.LicenseExpirationDate
            });
        }

        /// <summary>
        /// Deletes a driver by its unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the driver to delete.</param>
        /// <returns>No content if deletion is successful, or an error message if the driver has related trips.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var driver = _repository.GetById(id);
            if (driver == null) return NotFound("Driver not found.");

            var hasTrips = _context.Trips.Any(t => t.DriverId == id);
            if (hasTrips)
                return BadRequest("Cannot delete driver because there are trips associated with this driver.");

            _repository.Remove(id);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

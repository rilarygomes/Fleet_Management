using FleetManagement.Application.DTOs;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FleetManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleRepository _repository;
        private readonly FleetManagementDbContext _context;

        public VehicleController(IVehicleRepository repository, FleetManagementDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// Retrieves all vehicles with optional filters by model and year.
        /// </summary>
        /// <param name="model">Optional filter by vehicle model.</param>
        /// <param name="year">Optional filter by manufacturing year.</param>
        /// <returns>Returns a list of vehicles.</returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] string? model, [FromQuery] int? year)
        {
            var vehicles = _repository.GetAll();

            if (!string.IsNullOrEmpty(model))
                vehicles = vehicles.Where(v => v.Model.Contains(model, StringComparison.OrdinalIgnoreCase));

            if (year.HasValue)
                vehicles = vehicles.Where(v => v.Year == year.Value);

            return Ok(vehicles.Select(v => new VehicleDto
            {
                Id = v.Id,
                LicensePlate = v.LicensePlate,
                Model = v.Model,
                Year = v.Year
            }));
        }

        /// <summary>
        /// Retrieves a vehicle by its unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the vehicle.</param>
        /// <returns>Returns the vehicle information if found.</returns>
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var v = _repository.GetById(id);
            if (v == null) return NotFound();

            return Ok(new VehicleDto { Id = v.Id, LicensePlate = v.LicensePlate, Model = v.Model, Year = v.Year });
        }

        /// <summary>
        /// Creates a new vehicle.
        /// </summary>
        /// <param name="dto">Vehicle data transfer object containing license plate, model, and year.</param>
        /// <returns>Returns the created vehicle information.</returns>
        [HttpPost]
        public IActionResult Create([FromBody] VehicleDto dto)
        {
            var vehicle = new Vehicle(Guid.NewGuid(), dto.LicensePlate, dto.Model, dto.Year);
            _repository.Add(vehicle);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = vehicle.Id }, dto);
        }

        /// <summary>
        /// Updates an existing vehicle.
        /// </summary>
        /// <param name="id">Unique identifier of the vehicle to update.</param>
        /// <param name="dto">Vehicle data transfer object containing updated values.</param>
        /// <returns>Returns the updated vehicle information.</returns>
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] UpdateVehicleDto dto)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return NotFound("Vehicle not found.");

            if (dto.Year > DateTime.Now.Year)
                return BadRequest("Manufacturing year cannot be in the future.");

            existing.Update(dto.LicensePlate, dto.Model, dto.Year);

            _repository.Update(existing);
            _context.SaveChanges();

            return Ok(new VehicleDto
            {
                Id = existing.Id,
                LicensePlate = existing.LicensePlate,
                Model = existing.Model,
                Year = existing.Year
            });
        }

        /// <summary>
        /// Deletes a vehicle by its unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the vehicle to delete.</param>
        /// <returns>No content if deletion is successful.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var vehicle = _repository.GetById(id);
            if (vehicle == null) return NotFound("Vehicle not found.");

            var hasTrips = _context.Trips.Any(t => t.VehicleId == id);
            if (hasTrips)
                return BadRequest("Cannot delete vehicle because there are trips associated with this vehicle.");

            _repository.Remove(id);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

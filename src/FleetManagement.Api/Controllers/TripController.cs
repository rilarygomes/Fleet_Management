using FleetManagement.Application.DTOs;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FleetManagement.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FleetManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripController : ControllerBase
    {
        private readonly ITripRepository _repository;
        private readonly FleetManagementDbContext _context;

        public TripController(ITripRepository repository, FleetManagementDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// Retrieves all trips with optional filters by start and end dates.
        /// </summary>
        /// <param name="startDate">Optional filter by trip start date.</param>
        /// <param name="endDate">Optional filter by trip end date.</param>
        /// <returns>Returns a list of trips.</returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var trips = _repository.GetAll();

            if (startDate.HasValue)
                trips = trips.Where(t => t.StartDate >= startDate.Value);

            if (endDate.HasValue)
                trips = trips.Where(t => t.EndDate <= endDate.Value);

            return Ok(trips.Select(t => new TripDto
            {
                Id = t.Id,
                VehicleId = t.VehicleId,
                DriverId = t.DriverId,
                StartDate = t.StartDate,
                EndDate = t.EndDate
            }));
        }

        /// <summary>
        /// Retrieves a trip by its unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the trip.</param>
        /// <returns>Returns the trip information if found.</returns>
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var t = _repository.GetById(id);
            if (t == null) return NotFound();

            return Ok(new TripDto { Id = t.Id, VehicleId = t.VehicleId, DriverId = t.DriverId, StartDate = t.StartDate, EndDate = t.EndDate });
        }

        /// <summary>
        /// Creates a new trip.
        /// </summary>
        /// <param name="dto">Trip data transfer object containing vehicle, driver, start date, and end date.</param>
        /// <returns>Returns the created trip information.</returns>
        [HttpPost]
        public IActionResult Create([FromBody] TripDto dto)
        {
            var trip = new Trip(Guid.NewGuid(), dto.VehicleId, dto.DriverId, dto.StartDate, dto.EndDate);
            _repository.Add(trip);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = trip.Id }, dto);
        }

        /// <summary>
        /// Updates an existing trip.
        /// </summary>
        /// <param name="id">Unique identifier of the trip to update.</param>
        /// <param name="dto">Trip data transfer object containing updated values.</param>
        /// <param name="validator"></param>
        /// <returns>Returns the updated trip information.</returns>
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] UpdateTripDto dto, [FromServices] IValidator<UpdateTripDto> validator)
        {
            var existing = _repository.GetById(id);
            if (existing == null) return NotFound("Trip not found.");

            var context = new ValidationContext<UpdateTripDto>(dto);
            context.RootContextData["ExistingTrip"] = new TripDto
            {
                Id = existing.Id,
                VehicleId = existing.VehicleId,
                DriverId = existing.DriverId,
                StartDate = existing.StartDate,
                EndDate = existing.EndDate
            };

            var result = validator.Validate(context);
            if (!result.IsValid)
                return BadRequest(result.Errors.Select(e => e.ErrorMessage));

            existing.Update(dto.VehicleId, dto.DriverId, dto.StartDate, dto.EndDate);

            _repository.Update(existing);
            _context.SaveChanges();

            return Ok(new TripDto
            {
                Id = existing.Id,
                VehicleId = existing.VehicleId,
                DriverId = existing.DriverId,
                StartDate = existing.StartDate,
                EndDate = existing.EndDate
            });
        }

        /// <summary>
        /// Deletes a trip by its unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the trip to delete.</param>
        /// <returns>No content if deletion is successful.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _repository.Remove(id);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

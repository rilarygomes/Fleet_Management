using FleetManagement.Application.DTOs;
using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;
using FluentValidation;

namespace FleetManagement.Application.Services
{
    public class TripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly IValidator<TripDto> _validator;

        public TripService(ITripRepository tripRepository, IValidator<TripDto> validator)
        {
            _tripRepository = tripRepository;
            _validator = validator;
        }

        public IEnumerable<TripDto> GetAll()
        {
            return _tripRepository.GetAll()
                .Select(t => new TripDto
                {
                    Id = t.Id,
                    VehicleId = t.VehicleId,
                    DriverId = t.DriverId,
                    StartDate = t.StartDate,
                    EndDate = t.EndDate
                });
        }

        public TripDto? GetById(Guid id)
        {
            var trip = _tripRepository.GetById(id);
            return trip == null ? null : new TripDto
            {
                Id = trip.Id,
                VehicleId = trip.VehicleId,
                DriverId = trip.DriverId,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate
            };
        }

        public void Add(TripDto dto)
        {
            _validator.ValidateAndThrow(dto);

            var trip = new Trip(
                Guid.NewGuid(),          
                dto.VehicleId,
                dto.DriverId,
                dto.StartDate,
                dto.EndDate
            );

            _tripRepository.Add(trip);
        }

        public void Update(TripDto dto)
        {
            _validator.ValidateAndThrow(dto);

            var trip = new Trip(
                dto.Id,                  
                dto.VehicleId,
                dto.DriverId,
                dto.StartDate,
                dto.EndDate
            );

            _tripRepository.Update(trip);
        }

        public void Remove(Guid id) => _tripRepository.Remove(id);
    }
}

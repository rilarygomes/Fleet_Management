using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;

namespace FleetManagement.Infrastructure.Persistence.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly FleetManagementDbContext _context;

        public TripRepository(FleetManagementDbContext context)
        {
            _context = context;
        }

        public Trip? GetById(Guid id) =>
            _context.Trips.Find(id);

        public IEnumerable<Trip> GetTripsByVehicle(Guid vehicleId) =>
            _context.Trips.Where(t => t.VehicleId == vehicleId).ToList();

        public IEnumerable<Trip> GetTripsByDriver(Guid driverId) =>
            _context.Trips.Where(t => t.DriverId == driverId).ToList();

        public IEnumerable<Trip> GetAll() =>
            _context.Trips.ToList();

        public void Add(Trip trip) =>
            _context.Trips.Add(trip);

        public void Update(Trip trip) =>
            _context.Trips.Update(trip);

        public void Remove(Guid id)
        {
            var trip = _context.Trips.Find(id);
            if (trip != null)
                _context.Trips.Remove(trip);
        }
    }
}

using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;

namespace FleetManagement.Infrastructure.Persistence.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly FleetManagementDbContext _context;

        public VehicleRepository(FleetManagementDbContext context)
        {
            _context = context;
        }

        public Vehicle? GetById(Guid id) =>
            _context.Vehicles.Find(id);

        public Vehicle? GetByLicensePlate(string licensePlate) =>
            _context.Vehicles.FirstOrDefault(v => v.LicensePlate == licensePlate);

        public IEnumerable<Vehicle> GetAll() =>
            _context.Vehicles.ToList();
        public bool HasTrips(Guid vehicleId)
        {
            return _context.Trips.Any(t => t.VehicleId == vehicleId);
        }

        public void Add(Vehicle vehicle) =>
            _context.Vehicles.Add(vehicle);

        public void Update(Vehicle vehicle) =>
            _context.Vehicles.Update(vehicle);

        public void Remove(Guid id)
        {
            var vehicle = _context.Vehicles.Find(id);
            if (vehicle != null)
                _context.Vehicles.Remove(vehicle);
        }
        public void SaveChanges() => _context.SaveChanges();
    }
}

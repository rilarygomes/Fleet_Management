using FleetManagement.Domain.Entities;
using FleetManagement.Domain.Repositories;

namespace FleetManagement.Infrastructure.Persistence.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly FleetManagementDbContext _context;

        public DriverRepository(FleetManagementDbContext context)
        {
            _context = context;
        }

        public Driver? GetById(Guid id) =>
            _context.Drivers.Find(id);

        public Driver? GetByLicenseNumber(string licenseNumber) =>
            _context.Drivers.FirstOrDefault(d => d.LicenseNumber == licenseNumber);

        public IEnumerable<Driver> GetAll() =>
            _context.Drivers.ToList();

        public void Add(Driver driver) =>
            _context.Drivers.Add(driver);

        public void Update(Driver driver) =>
            _context.Drivers.Update(driver);

        public void Remove(Guid id)
        {
            var driver = _context.Drivers.Find(id);
            if (driver != null)
                _context.Drivers.Remove(driver);
        }

        public bool HasTrips(Guid driverId) =>
            _context.Trips.Any(t => t.DriverId == driverId);

        public void SaveChanges() => _context.SaveChanges();

    }
}

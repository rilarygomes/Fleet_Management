namespace FleetManagement.Domain.Repositories
{
    using FleetManagement.Domain.Entities;

    public interface IVehicleRepository
    {
        IEnumerable<Vehicle> GetAll();
        Vehicle? GetById(Guid id);
        Vehicle? GetByLicensePlate(string licensePlate);
        bool HasTrips(Guid vehicleId); 
        void Add(Vehicle vehicle);
        void Update(Vehicle vehicle);
        void Remove(Guid id);
        void SaveChanges();
    }
}
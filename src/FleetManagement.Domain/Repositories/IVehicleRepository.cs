namespace FleetManagement.Domain.Repositories
{
    using FleetManagement.Domain.Entities;

    public interface IVehicleRepository
    {
        Vehicle? GetById(Guid id);
        Vehicle? GetByLicensePlate(string licensePlate);
        IEnumerable<Vehicle> GetAll();
        void Add(Vehicle vehicle);
        void Update(Vehicle vehicle);
        void Remove(Guid id);
    }
}
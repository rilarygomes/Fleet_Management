namespace FleetManagement.Domain.Repositories
{
    using FleetManagement.Domain.Entities;

    public interface ITripRepository
    {
        Trip? GetById(Guid id);
        IEnumerable<Trip> GetTripsByVehicle(Guid vehicleId);
        IEnumerable<Trip> GetTripsByDriver(Guid driverId);
        IEnumerable<Trip> GetAll();
        void Add(Trip trip);
        void Update(Trip trip);
        void Remove(Guid id);
    }
}

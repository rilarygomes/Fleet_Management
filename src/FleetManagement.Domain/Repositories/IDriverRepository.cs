namespace FleetManagement.Domain.Repositories
{
    using FleetManagement.Domain.Entities;

    public interface IDriverRepository
    {
        Driver? GetById(Guid id);
        Driver? GetByLicenseNumber(string licenseNumber);
        IEnumerable<Driver> GetAll();
        void Add(Driver driver);
        void Update(Driver driver);
        void Remove(Guid id);
    }
}

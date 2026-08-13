namespace FleetManagement.Domain.Entities
{
    public class Driver
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string LicenseNumber { get; private set; }
        public DateTime LicenseExpirationDate { get; private set; }

        public Driver(Guid id, string name, string licenseNumber, DateTime licenseExpirationDate)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Driver Id is required.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Driver name is required.");

            if (name.Length < 3)
                throw new ArgumentException("Driver name must have at least 3 characters.");

            if (string.IsNullOrWhiteSpace(licenseNumber))
                throw new ArgumentException("License number is required.");

            if (licenseNumber.Length < 5)
                throw new ArgumentException("License number must be at least 5 characters.");

            if (licenseExpirationDate <= DateTime.UtcNow.Date)
                throw new InvalidOperationException("Driver license is expired.");

            Id = id;
            Name = name;
            LicenseNumber = licenseNumber;
            LicenseExpirationDate = licenseExpirationDate;
        }

        public Driver() { }
    }
}

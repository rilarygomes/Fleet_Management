namespace FleetManagement.Domain.Entities
{
    public class Driver
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string LicenseNumber { get; set; }
        public DateTime LicenseExpirationDate { get; set; }

        public Driver(string name, string licenseNumber, DateTime licenseExpirationDate)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Driver name is required.");

            if (string.IsNullOrWhiteSpace(licenseNumber))
                throw new ArgumentException("License number is required.");

            if (licenseExpirationDate <= DateTime.UtcNow)
                throw new InvalidOperationException("Driver license is expired.");

            Id = Guid.NewGuid();
            Name = name;
            LicenseNumber = licenseNumber;
            LicenseExpirationDate = licenseExpirationDate;
        }
    }
}

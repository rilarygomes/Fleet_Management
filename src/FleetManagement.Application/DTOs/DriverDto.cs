namespace FleetManagement.Application.DTOs
{
    /// <summary>
    /// Represents a driver in the fleet.
    /// </summary>
    public class DriverDto
    {
        /// <summary>
        /// Unique identifier of the driver.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Full name of the driver (minimum 3 characters).
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Driver's license number (must be exactly 11 characters).
        /// </summary>
        public required string LicenseNumber { get; set; }

        /// <summary>
        /// Expiration date of the driver's license.
        /// </summary>
        public DateTime LicenseExpirationDate { get; set; }
    }
}

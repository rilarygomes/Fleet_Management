namespace FleetManagement.Application.DTOs
{
    public class CreateDriverDto
    {
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

namespace FleetManagement.Application.Vehicles.DTOs
{
    /// <summary>
    /// Represents a vehicle in the fleet.
    /// </summary>
    public class VehicleDto
    {
        /// <summary>
        /// Unique identifier of the vehicle.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// License plate of the vehicle (must be 7 characters).
        /// </summary>
        public required string LicensePlate { get; set; }

        /// <summary>
        /// Model name of the vehicle (minimum 2 characters).
        /// </summary>
        public required string Model { get; set; }

        /// <summary>
        /// Manufacturing year of the vehicle (between 1960 and next year).
        /// </summary>
        public int Year { get; set; }
    }
}

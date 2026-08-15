namespace FleetManagement.Application.DTOs
{
    public class CreateTripDto
    {
        /// <summary>
        /// Unique identifier of the vehicle assigned to the trip.
        /// </summary>
        public required Guid VehicleId { get; set; }

        /// <summary>
        /// Unique identifier of the driver assigned to the trip.
        /// </summary>
        public required Guid DriverId { get; set; }

        /// <summary>
        /// Start date and time of the trip (must not be in the past).
        /// </summary>
        public required DateTime StartDate { get; set; }

        /// <summary>
        /// End date and time of the trip (must be greater than or equal to StartDate).
        /// </summary>
        public required DateTime EndDate { get; set; }
    }
}

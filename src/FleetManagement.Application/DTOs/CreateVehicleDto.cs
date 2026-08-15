namespace FleetManagement.Application.DTOs
{
    public class CreateVehicleDto
    {
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

namespace FleetManagement.Application.DTOs
{
    public class UpdateTripDto
    {
        public Guid VehicleId { get; set; }
        public Guid DriverId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

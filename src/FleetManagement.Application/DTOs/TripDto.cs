namespace FleetManagement.Application.DTOs
{
    public class TripDto
    {
        public Guid Id { get; set; }
        public required Guid VehicleId { get; set; }
        public required Guid DriverId { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
    }
}

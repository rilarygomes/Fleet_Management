namespace FleetManagement.Application.DTOs
{
    public class VehicleDto
    {
        public Guid Id { get; set; }
        public required string LicensePlate { get; set; } 
        public required string Model { get; set; } 
        public int Year { get; set; }
    }
}

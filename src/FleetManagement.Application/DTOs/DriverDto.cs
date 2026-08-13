namespace FleetManagement.Application.DTOs
{
    public class DriverDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } 
        public required string LicenseNumber { get; set; } 
        public DateTime LicenseExpirationDate { get; set; }
    }
}

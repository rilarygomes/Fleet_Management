namespace FleetManagement.Application.Drivers.DTOs
{
    public class UpdateDriverDto
    {
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseExpirationDate { get; set; }
    }
}

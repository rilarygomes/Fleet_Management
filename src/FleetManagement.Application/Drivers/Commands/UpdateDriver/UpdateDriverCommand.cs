namespace FleetManagement.Application.Drivers.Commands.UpdateDriver;

public class UpdateDriverCommand
{

    public string Name { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public DateTime LicenseExpirationDate { get; set; }
}
namespace FleetManagement.Application.Drivers.Commands.CreateDriver;

public class CreateDriverCommand
{
    public string Name { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public DateTime LicenseExpirationDate { get; set; }
}
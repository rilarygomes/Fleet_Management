namespace FleetManagement.Application.Vehicles.Commands.CreateVehicle;

public class CreateVehicleCommand
{
    public string LicensePlate { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }
}
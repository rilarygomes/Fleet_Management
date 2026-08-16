namespace FleetManagement.Application.Vehicles.Commands.UpdateVehicle;

public class UpdateVehicleCommand
{
    public string LicensePlate { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }
}
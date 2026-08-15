namespace FleetManagement.Application.Trips.Commands.CreateTrip;

public class CreateTripCommand
{
    public Guid VehicleId { get; set; }

    public Guid DriverId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
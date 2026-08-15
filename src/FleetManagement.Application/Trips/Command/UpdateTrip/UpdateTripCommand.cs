namespace FleetManagement.Application.Trips.Commands.UpdateTrip;

public class UpdateTripCommand
{
    public Guid Id { get; set; }

    public Guid VehicleId { get; set; }

    public Guid DriverId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
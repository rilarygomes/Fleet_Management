namespace FleetManagement.Application.Trips.GetTrips;

public class GetTripsQuery
{
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public Guid? DriverId { get; set; }

    public Guid? VehicleId { get; set; }
}
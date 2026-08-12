namespace FleetManagement.Domain.Entities
{
    public class Trip
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Guid DriverId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Trip(Guid vehicleId, Guid driverId, DateTime startDate, DateTime endDate)
        {
            if (endDate <= startDate)
                throw new ArgumentException("Trip end date must be after start date.");

            if (endDate < DateTime.UtcNow)
                throw new ArgumentException("End date must be after today.");

            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            DriverId = driverId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}

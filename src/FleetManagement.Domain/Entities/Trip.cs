namespace FleetManagement.Domain.Entities
{
    public class Trip
    {
        public Guid Id { get; private set; }
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public Trip(Guid id, Guid vehicleId, Guid driverId, DateTime startDate, DateTime endDate)
        {
            if (vehicleId == Guid.Empty)
                throw new ArgumentException("VehicleId is required.");

            if (driverId == Guid.Empty)
                throw new ArgumentException("DriverId is required.");

            if (startDate < DateTime.UtcNow.Date)
                throw new ArgumentException("Start date cannot be in the past.");

            if (endDate < startDate)
                throw new ArgumentException("Trip end date must be after or equal to start date.");

            Id = id;
            VehicleId = vehicleId;
            DriverId = driverId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}

namespace FleetManagement.Domain.Exceptions
{
    public class TripOverlapException : Exception
    {
        public TripOverlapException(Guid vehicleId)
            : base($"The vehicle with ID '{vehicleId}' already has a trip scheduled during this period.") { }
    }
}

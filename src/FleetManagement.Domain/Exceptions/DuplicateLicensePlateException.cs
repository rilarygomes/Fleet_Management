namespace FleetManagement.Domain.Exceptions
{
    public class DuplicateLicensePlateException : Exception
    {
        public DuplicateLicensePlateException(string licensePlate)
            : base($"The license plate '{licensePlate}' is already registered.") { }
    }
}

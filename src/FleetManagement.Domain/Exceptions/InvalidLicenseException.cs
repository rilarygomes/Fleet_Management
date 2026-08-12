namespace FleetManagement.Domain.Exceptions
{
    public class InvalidLicenseException : Exception
    {
        public InvalidLicenseException(string licenseNumber)
            : base($"The driver license '{licenseNumber}' is invalid.") { }
    }
}

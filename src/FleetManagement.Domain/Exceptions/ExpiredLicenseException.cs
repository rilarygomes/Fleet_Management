namespace FleetManagement.Domain.Exceptions
{
    public class ExpiredLicenseException : Exception
    {
        public ExpiredLicenseException(string licenseNumber)
            : base($"The driver license '{licenseNumber}' is expired.") { }
    }
}

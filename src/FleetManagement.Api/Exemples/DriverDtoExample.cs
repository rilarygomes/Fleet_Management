using Swashbuckle.AspNetCore.Filters;
using FleetManagement.Application.DTOs;

public class DriverDtoExample : IExamplesProvider<DriverDto>
{
    public DriverDto GetExamples()
    {
        return new DriverDto
        {
            Name = "John Doe",
            LicenseNumber = "ABC12345678",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(5)
        };
    }
}

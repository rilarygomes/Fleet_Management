using FleetManagement.Application.Drivers.Commands.UpdateDriver;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Swagger;

public class UpdateDriverCommandExample : IExamplesProvider<UpdateDriverCommand>
{
    public UpdateDriverCommand GetExamples()
    {
        return new UpdateDriverCommand
        {
            Name = "John Doe",
            LicenseNumber = "ABC12345678",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(5)
        };
    }
}
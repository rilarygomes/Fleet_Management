using FleetManagement.Application.Drivers.Commands.CreateDriver;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Swagger;

public class CreateDriverCommandExample : IExamplesProvider<CreateDriverCommand>
{
    public CreateDriverCommand GetExamples()
    {
        return new CreateDriverCommand
        {
            Name = "John Doe",
            LicenseNumber = "ABC12345678",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(5)
        };
    }
}
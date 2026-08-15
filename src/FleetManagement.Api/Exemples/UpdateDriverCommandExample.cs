using FleetManagement.Application.Drivers.Commands.UpdateDriver;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Swagger;

public class UpdateDriverCommandExample : IExamplesProvider<UpdateDriverCommand>
{
    public UpdateDriverCommand GetExamples()
    {
        return new UpdateDriverCommand
        {
            Id = Guid.Parse("9e47d9b3-9b9a-4b6e-95c4-cf79b4c6b3f7"),
            Name = "John Doe",
            LicenseNumber = "ABC12345678",
            LicenseExpirationDate = DateTime.UtcNow.AddYears(5)
        };
    }
}
using FleetManagement.Application.Vehicles.Commands.CreateVehicle;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Swagger;

public class CreateVehicleCommandExample : IExamplesProvider<CreateVehicleCommand>
{
    public CreateVehicleCommand GetExamples()
    {
        return new CreateVehicleCommand
        {
            LicensePlate = "ABC1234",
            Model = "Honda Civic",
            Year = 2024
        };
    }
}
using FleetManagement.Application.Vehicles.Commands.UpdateVehicle;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Swagger;

public class UpdateVehicleCommandExample : IExamplesProvider<UpdateVehicleCommand>
{
    public UpdateVehicleCommand GetExamples()
    {
        return new UpdateVehicleCommand
        {
            LicensePlate = "ABC1234",
            Model = "Honda Civic",
            Year = 2024
        };
    }
}
using FleetManagement.Application.Vehicles.Commands.UpdateVehicle;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Swagger;

public class UpdateVehicleCommandExample : IExamplesProvider<UpdateVehicleCommand>
{
    public UpdateVehicleCommand GetExamples()
    {
        return new UpdateVehicleCommand
        {
            Id = Guid.Parse("4a3d7e3d-1f48-4d71-b77c-5e5df3f7f7d9"),
            LicensePlate = "ABC1234",
            Model = "Honda Civic",
            Year = 2024
        };
    }
}
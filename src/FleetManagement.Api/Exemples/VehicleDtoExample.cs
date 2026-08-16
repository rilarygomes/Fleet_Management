using FleetManagement.Application.Vehicles.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Swagger;

public class VehicleDtoExample : IExamplesProvider<VehicleDto>
{
    public VehicleDto GetExamples()
    {
        return new VehicleDto
        {
            Id = Guid.Parse("4a3d7e3d-1f48-4d71-b77c-5e5df3f7f7d9"),
            LicensePlate = "ABC1234",
            Model = "Honda Civic",
            Year = 2025
        };
    }
}
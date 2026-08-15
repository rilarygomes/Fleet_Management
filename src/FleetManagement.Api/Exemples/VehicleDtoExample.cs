using Swashbuckle.AspNetCore.Filters;
using FleetManagement.Application.DTOs;

public class VehicleDtoExample : IExamplesProvider<VehicleDto>
{
    public VehicleDto GetExamples()
    {
        return new VehicleDto
        {
            LicensePlate = "XYZ1234",
            Model = "Civic",
            Year = 2025
        };
    }
}

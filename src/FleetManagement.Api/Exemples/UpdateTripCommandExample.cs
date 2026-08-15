using FleetManagement.Application.Trips.Commands.UpdateTrip;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Swagger;

public class UpdateTripCommandExample : IExamplesProvider<UpdateTripCommand>
{
    public UpdateTripCommand GetExamples()
    {
        return new UpdateTripCommand
        {
            Id = Guid.Parse("b7f8c91d-9d6e-4b53-ae86-2f0a9b79bcb3"),
            VehicleId = Guid.Parse("0d6c73bb-4c4b-4c6b-9a88-96f37b8fd8a1"),
            DriverId = Guid.Parse("7dfe1b59-6b9b-46f2-83af-5f65dd66c91f"),
            StartDate = DateTime.UtcNow.AddDays(5),
            EndDate = DateTime.UtcNow.AddDays(6)
        };
    }
}
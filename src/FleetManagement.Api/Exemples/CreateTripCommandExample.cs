using FleetManagement.Application.Trips.Commands.CreateTrip;
using Swashbuckle.AspNetCore.Filters;

namespace FleetManagement.Api.Swagger;

public class CreateTripCommandExample : IExamplesProvider<CreateTripCommand>
{
    public CreateTripCommand GetExamples()
    {
        return new CreateTripCommand
        {
            VehicleId = Guid.Parse("0d6c73bb-4c4b-4c6b-9a88-96f37b8fd8a1"),
            DriverId = Guid.Parse("7dfe1b59-6b9b-46f2-83af-5f65dd66c91f"),
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(3)
        };
    }
}
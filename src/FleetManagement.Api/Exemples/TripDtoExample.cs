using Swashbuckle.AspNetCore.Filters;
using FleetManagement.Application.DTOs;

public class TripDtoExample : IExamplesProvider<TripDto>
{
    public TripDto GetExamples()
    {
        return new TripDto
        {
            VehicleId = Guid.NewGuid(),
            DriverId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2)
        };
    }
}

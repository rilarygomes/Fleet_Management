using FleetManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registrations
            var descriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<FleetManagementDbContext>))
                .ToList();
            foreach (var d in descriptors) services.Remove(d);

            var dbContextDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(FleetManagementDbContext));
            if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

            // Register InMemory DbContext
            services.AddDbContext<FleetManagementDbContext>(options =>
                options.UseInMemoryDatabase("IntegrationTestDb"));
        });
    }
}

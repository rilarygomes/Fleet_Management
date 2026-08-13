using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FleetManagement.Infrastructure.Persistence
{
    public class FleetManagementDbContextFactory : IDesignTimeDbContextFactory<FleetManagementDbContext>
    {
        public FleetManagementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FleetManagementDbContext>();
            optionsBuilder.UseSqlite("Data Source=fleet.db");

            return new FleetManagementDbContext(optionsBuilder.Options);
        }
    }
}

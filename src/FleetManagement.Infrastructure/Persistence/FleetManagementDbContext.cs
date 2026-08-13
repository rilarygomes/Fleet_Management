using Microsoft.EntityFrameworkCore;
using FleetManagement.Domain.Entities;

namespace FleetManagement.Infrastructure.Persistence
{
    public class FleetManagementDbContext : DbContext
    {
        public FleetManagementDbContext(DbContextOptions<FleetManagementDbContext> options)
            : base(options) { }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FleetManagementDbContext).Assembly);
        }
    }
}

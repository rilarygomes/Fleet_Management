using FleetManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetManagement.Infrastructure.Persistence.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.ToTable("Trips");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.StartDate)
                   .IsRequired();

            builder.Property(t => t.EndDate)
                   .IsRequired();

            builder.HasOne<Vehicle>()
                   .WithMany()
                   .HasForeignKey(t => t.VehicleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Driver>()
                   .WithMany()
                   .HasForeignKey(t => t.DriverId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

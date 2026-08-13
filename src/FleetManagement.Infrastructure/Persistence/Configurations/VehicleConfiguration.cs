using FleetManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FleetManagement.Infrastructure.Persistence.Configurations
{
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles");
            builder.HasKey(v => v.Id);

            builder.Property(v => v.LicensePlate)
                   .IsRequired()
                   .HasMaxLength(7); 

            builder.Property(v => v.Model)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(v => v.Year)
                   .IsRequired();
        }
    }
}

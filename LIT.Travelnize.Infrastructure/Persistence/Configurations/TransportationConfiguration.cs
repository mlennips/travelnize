using LIT.Travelnize.Domain.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class AppDbContext
    {
        public class TransportationConfiguration : IEntityTypeConfiguration<Transportation>
        {
            public void Configure(EntityTypeBuilder<Transportation> builder)
            {
                builder.HasKey(x => x.Id);

                builder.OwnsOne(t => t.Departure, d =>
                {
                    d.OwnsOne(x => x.Address);
                    d.OwnsOne(x => x.Coordinates, c =>
                    {
                        c.Property(p => p.Latitude).HasColumnName("Departure_Latitude");
                        c.Property(p => p.Longitude).HasColumnName("Departure_Longitude");
                    });
                });

                builder.OwnsOne(t => t.Arrival, a =>
                {
                    a.OwnsOne(x => x.Address);
                    a.OwnsOne(x => x.Coordinates, c =>
                    {
                        c.Property(p => p.Latitude).HasColumnName("Arrival_Latitude");
                        c.Property(p => p.Longitude).HasColumnName("Arrival_Longitude");
                    });
                });

                builder.OwnsOne(t => t.RouteLink);
                builder.OwnsOne(t => t.Type);

                builder.HasMany(x => x.Passengers);
                builder.Navigation(t => t.Passengers).AutoInclude();
            }
        }
    }
}
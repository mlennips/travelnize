using LIT.Travelnize.Domain.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class AppDbContext
    {
        public class DestinationConfiguration : IEntityTypeConfiguration<Destination>
        {
            public void Configure(EntityTypeBuilder<Destination> builder)
            {
                builder.HasKey(x => x.Id);
                builder.OwnsOne(x => x.Slot);

                builder.OwnsOne(x => x.Location, l =>
                {
                    l.OwnsOne(x => x.Address);
                    l.OwnsOne(x => x.Coordinates, c =>
                    {
                        // Optional: sprechende Spaltennamen vergeben
                        c.Property(p => p.Latitude).HasColumnName("Location_Latitude");
                        c.Property(p => p.Longitude).HasColumnName("Location_Longitude");
                    });
                });

                builder.OwnsOne(x => x.ImageUrl);
                builder.OwnsOne(x => x.Website);

                builder.HasMany(x => x.Accommodations);
                builder.HasMany(x => x.Activities);

                builder.Navigation(t => t.Accommodations).AutoInclude();
                builder.Navigation(x => x.Activities).AutoInclude();
            }
        }
    }
}
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
                        c.Property(p => p.Latitude).HasColumnName("Location_Latitude");
                        c.Property(p => p.Longitude).HasColumnName("Location_Longitude");
                    });
                });

                builder.OwnsOne(x => x.Image, o =>
                {
                    o.Property(p => p.Name).HasColumnName("Image_Name");
                    o.Property(p => p.Source).HasColumnName("Image_Source");
                    o.Property(p => p.Kind).HasColumnName("Image_Kind");
                    o.Property(p => p.Thumbnail).HasColumnName("Image_Thumbnail");
                    o.Property(p => p.IsExternal).HasColumnName("Image_IsExternal");
                });

                builder.OwnsOne(x => x.Website, o =>
                {
                    o.Property(p => p.Name).HasColumnName("Website_Name");
                    o.Property(p => p.Source).HasColumnName("Website_Source");
                    o.Property(p => p.Kind).HasColumnName("Website_Kind");
                    o.Property(p => p.Thumbnail).HasColumnName("Website_Thumbnail");
                    o.Property(p => p.IsExternal).HasColumnName("Website_IsExternal");
                });

                builder.HasMany(x => x.Accommodations);
                builder.HasMany(x => x.Activities);

                builder.Navigation(t => t.Accommodations).AutoInclude();
                builder.Navigation(x => x.Activities).AutoInclude();
            }
        }
    }
}
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
                builder.ComplexProperty(x => x.Slot);
                builder.ComplexProperty(x => x.Location, l =>
                {
                    l.ComplexProperty(l => l.Coordinates);
                    l.ComplexProperty(l => l.Address);
                });
                builder.OwnsOne(x => x.Image);
                builder.OwnsOne(x => x.Website);
                builder.HasMany(x => x.Accommodations);
                builder.HasMany(x => x.Activities);
                builder.Navigation(x => x.Accommodations).AutoInclude();
                builder.Navigation(x => x.Activities).AutoInclude();
            }
        }
    }
}
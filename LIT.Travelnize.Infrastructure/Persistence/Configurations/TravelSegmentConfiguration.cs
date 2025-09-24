using LIT.Travelnize.Domain.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class AppDbContext
    {
        public class TravelSegmentConfiguration : IEntityTypeConfiguration<TravelSegment>
        {
            public void Configure(EntityTypeBuilder<TravelSegment> builder)
            {
                builder.HasKey(x => x.Id);
                builder.ComplexProperty(x => x.Slot);
                builder.HasMany(x => x.Destinations);
                builder.Navigation(x => x.Destinations).AutoInclude();
            }
        }
    }
}
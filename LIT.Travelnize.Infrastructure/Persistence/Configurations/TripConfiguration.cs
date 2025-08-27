using LIT.Travelnize.Domain.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class AppDbContext
    {
        public class TripConfiguration : IEntityTypeConfiguration<Trip>
        {
            public void Configure(EntityTypeBuilder<Trip> builder)
            {
                builder.HasKey(x => x.Id);
                builder.OwnsOne(x => x.TravelPeriod);
                builder.HasMany(x => x.TravelSegments);
                builder.HasMany(x => x.Transportations);
                builder.HasMany(x => x.Participants);
                builder.Navigation(x => x.Participants).AutoInclude();
                builder.Navigation(x => x.TravelSegments).AutoInclude();
                builder.Navigation(x => x.Transportations).AutoInclude();
            }
        }
    }
}
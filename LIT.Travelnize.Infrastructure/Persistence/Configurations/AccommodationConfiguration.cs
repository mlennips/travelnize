using LIT.Travelnize.Domain.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class AppDbContext
    {
        public class AccommodationConfiguration : IEntityTypeConfiguration<Accommodation>
        {
            public void Configure(EntityTypeBuilder<Accommodation> builder)
            {
                builder.HasKey(x => x.Id);

                builder.Property(a => a.TripId).IsRequired();

                builder.Property(a => a.TravelSegmentId).IsRequired();

                builder.Property(a => a.DestinationId).IsRequired();

                builder.ComplexProperty(a => a.Type);
                builder.ComplexProperty(a => a.Address);
                builder.ComplexProperty(a => a.CheckInOut);
                builder.ComplexProperty(a => a.BookingInfo, a =>
                {
                    a.ComplexProperty(a => a.Url);
                });
            }
        }
    }
}
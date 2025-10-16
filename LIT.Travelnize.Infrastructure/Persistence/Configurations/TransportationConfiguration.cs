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
                builder.ComplexProperty(x => x.Departure, l =>
                {
                    l.ComplexProperty(l => l.Coordinates);
                    l.ComplexProperty(l => l.Address);
                });
                builder.ComplexProperty(x => x.Arrival, l =>
                {
                    l.ComplexProperty(l => l.Coordinates);
                    l.ComplexProperty(l => l.Address);
                });
                builder.OwnsOne(t => t.RouteWebsite);
                builder.OwnsOne(t => t.TargetReference);
                builder.ComplexProperty(t => t.Type);
                builder.ComplexProperty(x => x.Traveltime);
                builder.HasMany(x => x.Passengers);
                builder.Navigation(x => x.Passengers).AutoInclude();
            }
        }
    }
}
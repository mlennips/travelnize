using LIT.Travelnize.Domain.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class AppDbContext
    {
        public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
        {
            public void Configure(EntityTypeBuilder<Activity> builder)
            {
                builder.HasKey(x => x.Id);
                builder.ComplexProperty(x => x.Location, l =>
                {
                    l.ComplexProperty(l => l.Coordinates);
                    l.ComplexProperty(l => l.Address);
                });
            }
        }
    }
}
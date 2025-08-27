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
                builder.OwnsOne(a => a.Type);
                builder.OwnsOne(a => a.Address);
            }
        }
    }
}
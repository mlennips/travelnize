using LIT.Travelnize.Domain.Trips;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class AppDbContext
    {
        public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
        {
            public void Configure(EntityTypeBuilder<Participant> builder)
            {
                builder.HasKey(x => x.Id);
                builder.OwnsOne(p => p.Email);
                builder.OwnsOne(p => p.PermissionLevel);
            }
        }
    }
}
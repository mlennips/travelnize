using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class AppDbContext : DbContext
    {
        protected AppDbContext() {}
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Trip> Trips => Set<Trip>();
        public DbSet<TravelSegment> TravelSegments => Set<TravelSegment>();
        public DbSet<Destination> Destinations => Set<Destination>();
        public DbSet<Participant> Participants => Set<Participant>();
        public DbSet<Accommodation> Accommodations => Set<Accommodation>();
        public DbSet<Transportation> Transportations => Set<Transportation>();
        public DbSet<Activity> Activities => Set<Activity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
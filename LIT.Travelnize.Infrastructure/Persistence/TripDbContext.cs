using LIT.Travelnize.Domain.Trips;
using Microsoft.EntityFrameworkCore;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public class TripDbContext : DbContext
    {
        protected TripDbContext()
        {
        }

        public TripDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<TravelSegment> TripSegments { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Transportation> Transportations { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var tripConfiguration = modelBuilder.Entity<Trip>();
            tripConfiguration.OwnsOne(x => x.TravelPeriod);

            var travelSegmentConfiguration = modelBuilder.Entity<TravelSegment>();
            travelSegmentConfiguration.OwnsOne(x => x.DateRange);

            var participantConfiguration = modelBuilder.Entity<Participant>();
            participantConfiguration.OwnsOne(x => x.Email);
            participantConfiguration.OwnsOne(x => x.PermissionLevel);

            var transportationConfiguration = modelBuilder.Entity<Transportation>();
            transportationConfiguration.OwnsOne(x => x.Departure);
            transportationConfiguration.OwnsOne(x => x.Arrival);
            transportationConfiguration.OwnsOne(x => x.RouteLink);
            transportationConfiguration.OwnsOne(x => x.Type);

            var destinationConfiguration = modelBuilder.Entity<Destination>();
            destinationConfiguration.OwnsOne(x => x.DateRange);
            destinationConfiguration.OwnsOne(x => x.Location);

            var AccommodationConfiguration = modelBuilder.Entity<Accommodation>();
            AccommodationConfiguration.OwnsOne(x => x.Address);

            var activityConfiguration = modelBuilder.Entity<Activity>();
            activityConfiguration.OwnsOne(x => x.Location);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }
    }
}
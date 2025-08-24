using LIT.Travelnize.Domain.Trips;
using Microsoft.EntityFrameworkCore;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    /// <summary>
    /// Run after changes to the model: dotnet ef migrations add [Message] --project LIT.Travelnize.Infrastructure --startup-project LIT.Travelnize.API
    /// Apply to database: dotnet ef database update --project LIT.Travelnize.Infrastructure --startup-project LIT.Travelnize.API
    public class TripContext : DbContext
    {
        protected TripContext()
        {
        }

        public TripContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<TravelSegment> TravelSegments { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Transportation> Transportations { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            BuildTrip(modelBuilder);
            BuildTravelSegment(modelBuilder);
            BuildDestination(modelBuilder);
            BuildParticipant(modelBuilder);
            BuildAccommodation(modelBuilder);
            BuildTransportation(modelBuilder);
            BuildActivity(modelBuilder);
        }

        private static void BuildTrip(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trip>(entity =>
            {
                entity.OwnsOne(t => t.TravelPeriod);
            });
        }

        private static void BuildTravelSegment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TravelSegment>(entity =>
            {
                entity.OwnsOne(t => t.DateRange);
            });
        }

        private static void BuildDestination(ModelBuilder modelBuilder)
        {
            var destinationConfiguration = modelBuilder.Entity<Destination>(entity =>
            {
                entity.OwnsOne(x => x.DateRange);
                entity.OwnsOne(x => x.Location, l =>
                {
                    l.OwnsOne(x => x.Address);
                });
                entity.OwnsOne(x => x.ImageUrl);
                entity.OwnsOne(x => x.Website);
            });
        }

        private static void BuildParticipant(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Participant>(entity =>
            {
                entity.OwnsOne(p => p.Email);
                entity.OwnsOne(p => p.PermissionLevel);
            });
        }

        private static void BuildAccommodation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accommodation>(entity =>
            {
                entity.OwnsOne(a => a.Type);
                entity.OwnsOne(a => a.Address);
            });
        }

        private static void BuildTransportation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transportation>(entity =>
            {
                entity.OwnsOne(t => t.Departure, d =>
                {
                    d.OwnsOne(x => x.Address);
                });
                entity.OwnsOne(t => t.Arrival, a =>
                {
                    a.OwnsOne(x => x.Address);
                });
                entity.OwnsOne(t => t.RouteLink);
                entity.OwnsOne(t => t.Type);
            });
        }

        private static void BuildActivity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activity>(entity =>
            {
                entity.OwnsOne(x => x.Location, l =>
                {
                    l.OwnsOne(x => x.Address);
                    l.OwnsOne(x => x.Coordinates);
                });
            });
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //}
    }
}
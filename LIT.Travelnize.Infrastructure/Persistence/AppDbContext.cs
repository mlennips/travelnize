using LIT.Travelnize.Domain.Trips;
using Microsoft.EntityFrameworkCore;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class AppDbContext : DbContext
    {
        protected AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
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
            // Run after changes to the model: dotnet ef migrations add [Message] --project LIT.Travelnize.Infrastructure --startup-project LIT.Travelnize.API
            // Apply to database: dotnet ef database update --project LIT.Travelnize.Infrastructure --startup-project LIT.Travelnize.API
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
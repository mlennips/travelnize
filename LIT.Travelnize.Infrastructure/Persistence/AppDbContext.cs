using LIT.Travelnize.Domain.Trips;
using Microsoft.EntityFrameworkCore;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    public partial class AppDbContext : DbContext
    {
        private readonly AuditSaveChangesInterceptor _auditInterceptor;

        protected AppDbContext() 
        {
            _auditInterceptor = default!;
        }

        public AppDbContext(DbContextOptions<AppDbContext> options, AuditSaveChangesInterceptor auditInterceptor)
            : base(options)
        {
            _auditInterceptor = auditInterceptor;
        }

        public DbSet<Trip> Trips => Set<Trip>();
        public DbSet<TravelSegment> TravelSegments => Set<TravelSegment>();
        public DbSet<Destination> Destinations => Set<Destination>();
        public DbSet<Participant> Participants => Set<Participant>();
        public DbSet<Accommodation> Accommodations => Set<Accommodation>();
        public DbSet<Transportation> Transportations => Set<Transportation>();
        public DbSet<Activity> Activities => Set<Activity>();
        public DbSet<AuditLogEntry> AuditLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditInterceptor);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
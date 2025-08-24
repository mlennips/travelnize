using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Destination : IEntity
    {
        private readonly List<Accommodation> _accommodations = [];

        public Guid Id { get; init; }
        public Guid TripId { get; init; }
        public Guid TravelSegmentId { get; private set; }
        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public DateRange DateRange { get; private set; } = default!;
        public Location Location { get; private set; } = default!;
        public ExternalUrl? ImageUrl { get; private set; }
        public ExternalUrl? Website { get; private set; }
        public IReadOnlyCollection<Accommodation> Accommodations { get => _accommodations.AsReadOnly(); init => _accommodations = value.ToList(); }

        internal static Destination Create(Guid tripId, Guid travelSegmentId, string name, string description,
            DateRange dateRange, Location location, ExternalUrl? imageUrl = null, ExternalUrl? url = null)
        {
            return new Destination()
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                TravelSegmentId = travelSegmentId,
                Name = name,
                Description = description,
                DateRange = dateRange,
                Location = location,
                ImageUrl = imageUrl,
                Website = url,
                Accommodations = []
            };
        }

        internal Result Update(string name, string description, Location location)
        {
            Name = name; 
            Description = description;
            Location = location;
            return Result.Success();
        }
    }
}

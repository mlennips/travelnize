using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Domain.Trips
{
    public class Destination : IEntity
    {
        private readonly List<Accommodation> _accommodations = [];
        private readonly List<Activity> _activities = [];

        public Guid Id { get; init; }
        public Guid TripId { get; init; }
        public Guid TravelSegmentId { get; private set; }

        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public PlanningSlot Slot { get; private set; } = default!;
        public Location Location { get; private set; } = default!;
        public ExternalUrl? ImageUrl { get; private set; }
        public ExternalUrl? Website { get; private set; }

        public IEnumerable<Accommodation> Accommodations { get => _accommodations.OrderBy(a => a.CheckIn); init => _accommodations = value.ToList(); }
        public IEnumerable<Activity> Activities { get => _activities.AsReadOnly(); init => _activities = value.ToList(); }

        internal static Destination Create(Guid tripId, Guid travelSegmentId, string name, string description,
            PlanningSlot slot, Location location, ExternalUrl? imageUrl = null, ExternalUrl? url = null)
        {
            return new Destination()
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                TravelSegmentId = travelSegmentId,
                Name = name,
                Description = description,
                Slot = slot,
                Location = location,
                ImageUrl = imageUrl,
                Website = url,
                Accommodations = [],
                Activities = []
            };
        }

        internal Result Update(string name, string description, Location location)
        {
            Name = name; 
            Description = description;
            Location = location;
            return Result.Success();
        }

        internal Result AddAccommodation(Accommodation accommodation)
        {
            _accommodations.Add(accommodation);
            return Result.Success();
        }
    }
}

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
        public ResourceReference? Image { get; private set; }
        public ResourceReference? Website { get; private set; }

        public IEnumerable<Accommodation> Accommodations { get => _accommodations.OrderBy(a => a.CheckInOut.Start); init => _accommodations = value.ToList(); }
        public IEnumerable<Activity> Activities { get => _activities.OrderBy(a => a.Date); init => _activities = value.ToList(); }

        internal static Destination Create(Guid tripId, Guid travelSegmentId, string name, string description,
            PlanningSlot slot, Location location, ResourceReference? image = null, ResourceReference? website = null)
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
                Image = image?.VerifyKind(ResourceKind.Image),
                Website = website?.VerifyKind(ResourceKind.Website),
                Accommodations = [],
                Activities = []
            };
        }

        internal Result Update(string name, string description, Location location, ResourceReference? image, ResourceReference? website)
        {
            Name = name; 
            Description = description;
            Location = location;
            Image = image?.VerifyKind(ResourceKind.Image);
            Website = website?.VerifyKind(ResourceKind.Website);
            return Result.Success();
        }

        internal Result AddAccommodation(Accommodation accommodation)
        {
            _accommodations.Add(accommodation);
            return Result.Success();
        }

        internal Result AddActivity(Activity activity)
        {
            _activities.Add(activity);
            return Result.Success();
        }
    }
}

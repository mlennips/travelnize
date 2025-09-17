using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Domain.Trips
{
    public class TravelSegment : IEntity
    {
        private readonly List<Destination> _destinations = [];

        public Guid Id { get; init; }
        public Guid TripId { get; init; }

        public string Description { get; private set; } = default!;
        public PlanningSlot Slot { get; private set; } = default!;
        public IEnumerable<Destination> Destinations => _destinations.OrderBy(x => x.Slot);

        internal static TravelSegment Create(Guid tripId, string description, PlanningSlot slot)
        {
            return new TravelSegment()
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                Description = description,
                Slot = slot
            };
        }

        internal void Update(PlanningSlot slot, string description)
        {
            Slot = slot;
            Description = description;
        }

        internal Result AddDestination(Destination destination)
        {
            if (Destinations.Any(d => d.Id == destination.Id))
            {
                return TripErrors.DestinationAlreadyExistsInSegment;
            }
            _destinations.Add(destination);
            return Result.Success();
        }

        internal Result RemoveDestination(Guid destinationId)
        {
            var destination = Destinations.FirstOrDefault(d => d.Id == destinationId);
            if (destination == null)
            {
                return TripErrors.DestinationNotFound;
            }
            _destinations.Remove(destination);
            return Result.Success();
        }
    }
}
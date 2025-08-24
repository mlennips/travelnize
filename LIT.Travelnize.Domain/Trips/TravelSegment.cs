using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class TravelSegment : IEntity
    {
        private readonly List<Destination> _destinations = [];

        public Guid Id { get; init; }
        public Guid TripId { get; init; }

        public string Description { get; private set; } = default!;
        public DateRange DateRange { get; private set; } = default!;
        public IReadOnlyCollection<Destination> Destinations { get => _destinations.AsReadOnly() ; init => _destinations = value.ToList(); }

        internal static TravelSegment Create(Guid tripId, string description, DateRange dateRange)
        {
            return new TravelSegment()
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                Description = description,
                DateRange = dateRange,
                Destinations = []
            };
        }

        internal void Update(DateTime startDate, DateTime endDate, string description)
        {
            DateRange = new DateRange(startDate, endDate);
            Description = description;
        }

        internal Result AddDestination(Destination destination)
        {
            if (Destinations.Any(d => d.Id == destination.Id))
            {
                return TripsErrors.DestinationAlreadyExistsInSegment;
            }
            if (DateRange.Overlaps(destination.DateRange))
            {
                return TripsErrors.DestinationDateRangeOutOfSegmentRange;
            }
            _destinations.Add(destination);
            return Result.Success();
        }

        internal Result RemoveDestination(Guid destinationId)
        {
            var destination = Destinations.FirstOrDefault(d => d.Id == destinationId);
            if (destination == null)
            {
                return TripsErrors.DestinationNotFound;
            }
            _destinations.Remove(destination);
            return Result.Success();
        }
    }
}
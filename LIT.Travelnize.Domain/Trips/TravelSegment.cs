using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class TravelSegment(Guid id, Guid tripId, string description,
        List<Destination> destinations, DateRange dateRange) : IEntity
    {
        public Guid Id { get; } = id;
        public Guid TripId { get; } = tripId;

        public string Description { get; private set; } = description;
        public DateRange DateRange { get; private set; } = dateRange;
        public IReadOnlyCollection<Destination> Destinations => destinations.AsReadOnly();

        internal static TravelSegment Create(Guid tripId, string description, DateRange dateRange)
        {
            return new TravelSegment(Guid.NewGuid(), tripId, description, [], dateRange);
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
            destinations.Add(destination);
            return Result.Success();
        }

        internal Result RemoveDestination(Guid destinationId)
        {
            var destination = Destinations.FirstOrDefault(d => d.Id == destinationId);
            if (destination == null)
            {
                return TripsErrors.DestinationNotFound;
            }
            destinations.Remove(destination);
            return Result.Success();
        }
    }
}
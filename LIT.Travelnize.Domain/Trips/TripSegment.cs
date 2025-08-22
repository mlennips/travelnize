using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class TripSegment(Guid id, Guid tripId, Destination[] destinations, DateRange dateRange, 
        string description) : IEntity
    {
        public Guid Id { get; } = id;
        public Guid TripId { get; } = tripId;
        public Destination[] Destinations { get; private set; } = destinations;
        public DateRange DateRange { get; private set; } = dateRange;
        public string Description { get; private set; } = description;

        public static TripSegment Create(Guid tripId, DateRange dateRange, string description)
        {
            return new TripSegment(Guid.NewGuid(), tripId, [], dateRange, description);
        }

        public void Update(DateTime startDate, DateTime endDate, string description)
        {
            DateRange = new DateRange(startDate, endDate);
            Description = description;
        }
    }
}
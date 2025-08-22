using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Activity(Guid id, Guid tripId, Guid tripSegmentId, Guid destinationId, string name, string description, 
        DateTime date, TimeSpan? time, Location? location) : IEntity
    {
        public Guid Id { get; } = id;
        public Guid TripId { get; } = tripId;
        public Guid TripSegmentId { get; private set; } = tripSegmentId;
        public Guid DestinationId { get; private set; } = destinationId;
        public string Name { get; private set; } = name;
        public string Description { get; private set; } = description;
        public DateTime Date { get; private set; } = date;
        public TimeSpan? Time { get; private set; } = time;
        public Location? Location { get; private set; } = location;

        public static Activity Create(Guid tripId, Guid tripSegmentId, Guid destinationId, string name, string description, 
            DateTime date, TimeSpan? time = null, Location? location = null)
        {
            return new Activity(Guid.NewGuid(), tripId, tripSegmentId, destinationId, name, description, date, time, location);
        }
    }
}

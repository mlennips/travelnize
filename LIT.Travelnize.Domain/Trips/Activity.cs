using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Activity(Guid id, Guid tripId, Guid travelSegmentId, Guid destinationId, string name, string description, 
        DateTime date, TimeSpan? time, Location? location) : IEntity
    {
        public Guid Id { get; } = id;
        public Guid TripId { get; } = tripId;
        public Guid TravelSegmentId { get; } = travelSegmentId;
        public Guid DestinationId { get; } = destinationId;

        public string Name { get; private set; } = name;
        public string Description { get; private set; } = description;
        public DateTime Date { get; private set; } = date;
        public TimeSpan? Time { get; private set; } = time;
        public Location? Location { get; private set; } = location;

        internal static Activity Create(Guid tripId, Guid travelSegmentId, Guid destinationId, string name, string description, 
            DateTime date, TimeSpan? time = null, Location? location = null)
        {
            return new Activity(Guid.NewGuid(), tripId, travelSegmentId, destinationId, name, description, date, time, location);
        }

        internal Result Update(string name, string description, 
            DateTime date, TimeSpan? time = null, Location? location = null)
        {
            Name = name;
            Description = description;
            Date = date;
            Time = time;
            Location = location;
            return Result.Success();
        }
    }
}

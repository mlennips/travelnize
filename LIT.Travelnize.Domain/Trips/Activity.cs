using System.ComponentModel.DataAnnotations.Schema;

namespace LIT.Travelnize.Domain.Trips
{
    public class Activity : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; init; }
        public Guid TripId { get; init; }
        public Guid TravelSegmentId { get; init; }
        public Guid DestinationId { get; init; }

        public string Name { get; private set; } = default!;
        public string Description { get; private set; } = default!;
        public DateTime? Date { get; private set; }
        public TimeSpan? Duration { get; private set; }
        public Location Location { get; private set; } = default!;

        internal static Activity Create(Guid tripId, Guid travelSegmentId, Guid destinationId, string name, string description,
            DateTime? date, TimeSpan? duration = null, Location? location = null)
        {
            return new Activity()
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                TravelSegmentId = travelSegmentId,
                DestinationId = destinationId,
                Name = name,
                Description = description,
                Date = date,
                Duration = duration,
                Location = location ?? Location.Empty
            };
        }

        internal Result Update(string name, string description, Location location,
            DateTime? date, TimeSpan? time = null)
        {
            Name = name;
            Description = description;
            Date = date;
            Duration = time;
            Location = location;

            return Result.Success();
        }
    }
}

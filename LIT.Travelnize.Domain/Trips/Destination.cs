using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Destination(Guid id, Guid tripId, Guid travelSegmentId, string name, string description,
        DateRange dateRange, Location location, List<Accommodation> accommodations, ExternalUrl? imageUrl, ExternalUrl? url) : IEntity
    {
        public Guid Id { get; } = id;
        public Guid TripId { get; } = tripId;
        public Guid TravelSegmentId { get; set; } = travelSegmentId;
        public string Name { get; private set; } = name;
        public string Description { get; private set; } = description;
        public DateRange DateRange { get; private set; } = dateRange;
        public Location Location { get; private set; } = location;
        public ExternalUrl? ImageUrl { get; private set; } = imageUrl;
        public ExternalUrl? Url { get; private set; } = url;
        public IReadOnlyCollection<Accommodation> Accommodations => accommodations.AsReadOnly();

        internal static Destination Create(Guid tripId, Guid travelSegmentId, string name, string description,
            DateRange dateRange, Location location, ExternalUrl? imageUrl = null, ExternalUrl? url = null)
        {
            return new Destination(Guid.NewGuid(), tripId, travelSegmentId, name, description, dateRange, location, [], imageUrl, url);
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

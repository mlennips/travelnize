using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Destination(Guid id, Guid tripId, Guid tripSegmentId, string name, string description,
        Location location, ExternalUrl? imageUrl, ExternalUrl? url) : IEntity
    {
        public Guid Id { get; } = id;
        public Guid TripId { get; } = tripId;
        public Guid TripSegmentId { get; set; } = tripSegmentId;
        public string Name { get; private set; } = name;
        public string Description { get; private set; } = description;
        public Location Location { get; private set; } = location;
        public ExternalUrl? ImageUrl { get; private set; } = imageUrl;
        public ExternalUrl? Url { get; private set; } = url;

        public static Destination Create(Guid tripId, Guid tripSegmentId, string name, string description,
            Location location, ExternalUrl? imageUrl = null, ExternalUrl? url = null)
        {
            return new Destination(Guid.NewGuid(), tripId, tripSegmentId, name, description, location, imageUrl, url);
        }
    }
}

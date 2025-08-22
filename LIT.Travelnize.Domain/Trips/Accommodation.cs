using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Accommodation(Guid id, Guid tripId, Guid tripSegmentId, string name, string accommodationType,
        Location address, DateTime checkIn, DateTime checkOut) : IEntity
    {
        public Guid Id { get; } = id;
        public Guid TripId { get; } = tripId;
        public Guid TripSegmentId { get; } = tripSegmentId;
        public string Name { get; private set; } = name;
        public string AccommodationType { get; private set; } = accommodationType;
        public Location Address { get; private set; } = address;
        public DateTime CheckIn { get; private set; } = checkIn;
        public DateTime CheckOut { get; private set; } = checkOut;

        public static Accommodation Create(Guid tripId, Guid tripSegmentId, string name, string accommodationType, 
            Location address, DateTime checkIn, DateTime checkOut)
        {
            return new Accommodation(Guid.NewGuid(), tripId, tripSegmentId, name, accommodationType, address, checkIn, checkOut);
        }
    }
}

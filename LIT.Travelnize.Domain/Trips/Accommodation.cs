using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Accommodation(Guid id, Guid tripId, Guid travelSegmentId, string name, AccommodationType type,
        Address address, DateTime checkIn, DateTime checkOut) : IEntity
    {
        public Guid Id { get; } = id;
        public Guid TripId { get; } = tripId;
        public Guid TravelSegmentId { get; } = travelSegmentId;

        public string Name { get; private set; } = name;
        public AccommodationType Type { get; private set; } = type;
        public Address Address { get; private set; } = address;
        public DateTime CheckIn { get; private set; } = checkIn;
        public DateTime CheckOut { get; private set; } = checkOut;

        internal static Accommodation Create(Guid tripId, Guid travelSegmentId, string name, AccommodationType type, 
            Address address, DateTime checkIn, DateTime checkOut)
        {
            return new Accommodation(Guid.NewGuid(), tripId, travelSegmentId, name, type, address, checkIn, checkOut);
        }

        internal Result Update(string name, AccommodationType type, Address address, DateTime checkIn, DateTime checkOut)
        {
            if (CheckIn >= CheckOut)
            {
                return TripsErrors.InvalidAccommodationDates;
            }

            Name = name;
            Type = type;
            Address = address;
            CheckIn = checkIn;
            CheckOut = checkOut;
            return Result.Success();
        }
    }
}

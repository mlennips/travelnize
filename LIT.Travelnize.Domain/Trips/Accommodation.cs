using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Trips
{
    public class Accommodation : IEntity
    {
        public Guid Id { get; init; }
        public Guid TripId { get; init; }
        public Guid TravelSegmentId { get; init; }

        public string Name { get; private set; } = default!;
        public AccommodationType Type { get; private set; } = default!;
        public Address Address { get; private set; } = default!;
        public DateTime CheckIn { get; private set; }
        public DateTime CheckOut { get; private set; }

        internal static Accommodation Create(Guid tripId, Guid travelSegmentId, string name, AccommodationType type, 
            Address address, DateTime checkIn, DateTime checkOut)
        {
            return new Accommodation()
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                TravelSegmentId = travelSegmentId,
                Name = name,
                Type = type,
                Address = address,
                CheckIn = checkIn,
                CheckOut = checkOut
            };
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

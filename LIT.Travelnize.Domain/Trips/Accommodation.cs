using LIT.Travelnize.Domain.Trips.ValueObjects;

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
        public PlanningSlot Slot { get; private set; } = default!;
        public DateTime? CheckIn => Slot.Start;
        public DateTime? CheckOut => Slot.End;

        internal static Accommodation Create(Guid tripId, Guid travelSegmentId, string name, AccommodationType type, 
            Address address, DateTime? checkIn, DateTime? checkOut)
        {
            return new Accommodation()
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                TravelSegmentId = travelSegmentId,
                Name = name,
                Type = type,
                Address = address,
                Slot = PlanningSlot.Create(checkIn, checkOut)
            };
        }

        internal Result Update(string name, AccommodationType type, Address address, DateTime checkIn, DateTime checkOut)
        {
            if (CheckIn >= CheckOut)
            {
                return TripErrors.InvalidAccommodationDates;
            }

            Name = name;
            Type = type;
            Address = address;
            Slot = PlanningSlot.Create(checkIn, checkOut);

            return Result.Success();
        }
    }
}

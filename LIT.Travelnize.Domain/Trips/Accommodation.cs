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
        public PlanningSlot CheckInOut { get; private set; } = default!;

        internal static Accommodation Create(Guid tripId, Guid travelSegmentId, string name, AccommodationType type, 
            Address address, PlanningSlot checkInOut)
        {
            return new Accommodation()
            {
                Id = Guid.NewGuid(),
                TripId = tripId,
                TravelSegmentId = travelSegmentId,
                Name = name,
                Type = type,
                Address = address,
                CheckInOut = checkInOut
            };
        }

        internal Result Update(string name, AccommodationType type, Address address, PlanningSlot checkInOut)
        {
            Name = name;
            Type = type;
            Address = address;
            CheckInOut = checkInOut;

            return Result.Success();
        }
    }
}

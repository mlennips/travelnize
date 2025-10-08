namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record AccommodationDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public AccommodationType Type { get; init; } = default!;
        public Address Address { get; init; } = default!;
        public PlanningSlot CheckInOut { get; init; } = default!;
        public BookingInfo BookingInfo { get; init; } = default!;

        public static AccommodationDto From(Accommodation accommodation)
        {
            return new AccommodationDto
            {
                Id = accommodation.Id,
                Name = accommodation.Name,
                Type = accommodation.Type,
                Address = accommodation.Address,
                CheckInOut = accommodation.CheckInOut,
                BookingInfo = accommodation.BookingInfo
            };
        }
    }
}
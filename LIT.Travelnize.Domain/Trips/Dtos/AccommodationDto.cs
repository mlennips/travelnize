using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record AccommodationDto
    {
        public Guid Id { get; init; }
        public Guid TripId { get; init; }
        public Guid TravelSegmentId { get; init; }
        public string Name { get; init; } = default!;
        public AccommodationType Type { get; init; } = default!;
        public AddressDto Address { get; init; } = default!;
        public DateTime? CheckIn { get; init; }
        public DateTime? CheckOut { get; init; }

        public static AccommodationDto From(Accommodation accommodation)
        {
            return new AccommodationDto
            {
                Id = accommodation.Id,
                Name = accommodation.Name,
                Type = accommodation.Type,
                Address = AddressDto.From(accommodation.Address),
                CheckIn = accommodation.CheckIn,
                CheckOut = accommodation.CheckOut
            };
        }
    }
}
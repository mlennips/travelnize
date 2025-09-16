namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record LocationDto(AddressDto Address)
    {
        public static LocationDto From(Location l) =>
            new(AddressDto.From(l.Address));
    }
}

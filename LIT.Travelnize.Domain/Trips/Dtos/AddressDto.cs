namespace LIT.Travelnize.Domain.Trips.Dtos
{
    public record AddressDto(
        string Line1,
        string Line2,
        string Street,
        string HouseNumber,
        string PostalCode,
        string City,
        string Country
    )
    {
        public static AddressDto From(Address a) =>
            new(a.Line1, a.Line2, a.Street, a.HouseNumber, a.PostalCode, a.City, a.Country);
    }
}

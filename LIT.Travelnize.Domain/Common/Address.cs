using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Common
{
    public record Address : IValueObject
    {
        public Address(string name, string street, string houseNumber, string postalCode, string city, string country)
        {
            Name = name;
            Street = street;
            HouseNumber = houseNumber;
            PostalCode = postalCode;
            City = city;
            Country = country;
        }

        public string Name { get; }
        public string Street { get; }
        public string HouseNumber { get; }
        public string PostalCode { get; }
        public string City { get; }
        public string Country { get; }

        public override string ToString()
        {
            return $"{Name}, {Street} {HouseNumber}, {PostalCode} {City}, {Country}";
        }

        public static Address Empty => new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
    }
}

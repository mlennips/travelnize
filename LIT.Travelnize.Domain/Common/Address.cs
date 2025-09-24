using LIT.Travelnize.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIT.Travelnize.Domain.Common
{
    public record Address : ValueObject
    {
        public Address() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
        {

        }

        public Address(string line1, string line2, string street, string houseNumber, string postalCode, string city, string country)
        {
            Line1 = line1;
            Line2 = line2;
            Street = street;
            HouseNumber = houseNumber;
            PostalCode = postalCode;
            City = city;
            Country = country;
        }

        public string Line1 { get; private init; }
        public string Line2 { get; private init; }
        public string Street { get; private init; }
        public string HouseNumber { get; private init; }
        public string PostalCode { get; private init; }
        public string City { get; private init; }
        public string Country { get; private init; }

        public override string ToString()
        {
            return $"{Line1}, {Street} {HouseNumber}, {PostalCode} {City}, {Country}";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Line1;
            yield return Street;
            yield return HouseNumber;
            yield return PostalCode;
            yield return City;
            yield return Country;
        }

        public static Address Empty => new();
    }
}

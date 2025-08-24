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

        public string Line1 { get; }
        public string Line2 { get; }
        public string Street { get; }
        public string HouseNumber { get; }
        public string PostalCode { get; }
        public string City { get; }
        public string Country { get; }

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

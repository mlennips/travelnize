using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Common
{
    public record Location : ValueObject
    {
        public Location(Address address, Coordinates? coordinates)
        {
            Address = address;
            Coordinates = coordinates;
        }

        public Address Address { get; }
        public Coordinates? Coordinates { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Address;
            if (Coordinates is not null)
            {
                yield return Coordinates;
            }
        }
    }
}

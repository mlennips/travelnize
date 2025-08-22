using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Common
{
    public record Location : IValueObject
    {
        public Location(Address address, Coordinates? coordinates)
        {
            Address = address;
            Coordinates = coordinates;
        }

        public Address Address { get; }
        public Coordinates? Coordinates { get; }
    }
}

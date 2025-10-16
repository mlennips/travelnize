namespace LIT.Travelnize.Domain.Common
{
    public record Location : ValueObject
    {
        private Location() : this(new Address(), new Coordinates()) { }

        public Location(Address address, Coordinates coordinates)
        {
            Address = address;
            Coordinates = coordinates;
        }

        public Address Address { get; private init; }
        public Coordinates Coordinates { get; private init; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Address;
            if (Coordinates is not null)
            {
                yield return Coordinates;
            }
        }

        public static Location Empty => new(Address.Empty, Coordinates.Empty);

        public static Location WithAddress(Address address) => new(address, Coordinates.Empty);

        public static Location WithCoordinates(double latitude, double longitude) => new(Address.Empty, new Coordinates { Latitude = latitude, Longitude = longitude });

        public static Location Create(Address address, Coordinates coordinates) => new(address, coordinates);
    }
}

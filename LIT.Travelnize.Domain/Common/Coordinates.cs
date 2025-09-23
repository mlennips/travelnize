namespace LIT.Travelnize.Domain.Common
{
    public record Coordinates : ValueObject
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public static Coordinates Empty => new() { Latitude = 0, Longitude = 0 };

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }

    }
}
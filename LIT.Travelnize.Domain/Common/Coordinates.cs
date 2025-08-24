using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Common
{
    public record Coordinates : ValueObject
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }
    }
}
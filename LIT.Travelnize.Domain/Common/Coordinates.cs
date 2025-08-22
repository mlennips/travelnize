using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Common
{
    public record Coordinates : IValueObject
    {
        public double Latitude { get; }
        public double Longitude { get; }
    }
}
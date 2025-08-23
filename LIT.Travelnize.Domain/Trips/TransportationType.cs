using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Trips
{
    public record TransportationType : SingleValueObject<string>
    {
        public TransportationType(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Transportation type cannot be empty.", nameof(value));
            }
            if (!AllowedTypes.Contains(value))
            {
                throw new ArgumentException($"Invalid transportation type. Allowed types are: {string.Join(", ", AllowedTypes)}", nameof(value));
            }
        }

        public static string[] AllowedTypes => ["Flight", "Train", "Bus", "Car", "Boat", "Bicycle", "Walk", "Other"];
    }
}
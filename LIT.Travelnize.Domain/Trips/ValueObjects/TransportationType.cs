namespace LIT.Travelnize.Domain.Trips.ValueObjects
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

        public static TransportationType Train => new("Train");
        public static TransportationType Flight => new("Flight");
        public static TransportationType Bus => new("Bus");
        public static TransportationType Car => new("Car");
        public static TransportationType Boat => new("Boat");
        public static TransportationType Bicycle => new("Bicycle");
        public static TransportationType Walk => new("Walk");
        public static TransportationType Other => new("Other");
    }
}
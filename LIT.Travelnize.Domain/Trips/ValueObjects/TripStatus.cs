namespace LIT.Travelnize.Domain.Trips.ValueObjects
{
    public record TripStatus : SingleValueObject<string>
    {
        public TripStatus(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Trip status cannot be empty.", nameof(value));
            }
        }

        public static TripStatus Upcoming => new("Upcoming");
        public static TripStatus Ongoing => new("Ongoing");
        public static TripStatus Past => new("Past");
        public static TripStatus Pending => new("Pending");

        public static TripStatus Create(DateTime? start, DateTime? end)
        {
            return start switch
            {
                null => Pending,
                _ when end == null || end > DateTime.UtcNow => start > DateTime.UtcNow ? Upcoming : Ongoing,
                _ when end <= DateTime.UtcNow => Past,
                _ => throw new InvalidOperationException("Invalid trip dates.")
            };
        }
    }
}

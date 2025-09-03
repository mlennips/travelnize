namespace LIT.Travelnize.Domain.Trips
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

        public static TripStatus Create(DateRange? dateRange) => dateRange switch
        {
            null => Pending,
            _ when dateRange.Start > DateTime.UtcNow => Upcoming,
            _ when dateRange.Start <= DateTime.UtcNow && dateRange.End >= DateTime.UtcNow => Ongoing,
            _ when dateRange.End < DateTime.UtcNow => Past,
            _ => throw new InvalidOperationException("Invalid date range")
        };


    }
}

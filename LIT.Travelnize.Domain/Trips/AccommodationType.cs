using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Trips
{
    public record AccommodationType : SingleValueObject<string>
    {
        public AccommodationType() : this("Other") { }

        public AccommodationType(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Accommodation type cannot be empty.");
            }
        }

        public static string[] DefaultValues =>
        [
            "Hotel",
            "Hostel",
            "Apartment",
            "Guesthouse",
            "Resort",
            "Bed and Breakfast",
            "Villa",
            "Cottage",
            "Motel",
            "Camping",
            "Cruise Ship",
            "Caravan",
            "Other"
        ];

        public static AccommodationType Default => new();
        public static AccommodationType Hotel => new("Hotel");
        public static AccommodationType Hostel => new("Hostel");
        public static AccommodationType Apartment => new("Apartment");
        public static AccommodationType Guesthouse => new("Guesthouse");
        public static AccommodationType Resort => new("Resort");
        public static AccommodationType BedAndBreakfast => new("Bed and Breakfast");
        public static AccommodationType Villa => new("Villa");
        public static AccommodationType Cottage => new("Cottage");
        public static AccommodationType Motel => new("Motel");
        public static AccommodationType Camping => new("Camping");
        public static AccommodationType CruiseShip => new("Cruise Ship");
        public static AccommodationType Caravan => new("Caravan");
        public static AccommodationType Other => new("Other");
    }
}
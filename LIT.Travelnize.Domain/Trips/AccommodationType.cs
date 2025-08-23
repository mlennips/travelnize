using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Trips
{
    public record AccommodationType : SingleValueObject<string>
    {
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

        public static AccommodationType Default => new("Other");
    }


}
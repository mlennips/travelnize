
namespace LIT.Travelnize.Domain.Trips.ValueObjects
{
    public record BookingInfo(
        string Provider,
        string BookingNumber,
        DateTime? BookingDate,
        ResourceReference Url,
        string Details
    ) : ValueObject
    {
        private BookingInfo() : this(string.Empty, string.Empty, null, ResourceReference.Empty, string.Empty)
        {
        }

        public static BookingInfo Empty => new();

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Provider;
            yield return BookingNumber;
            if (BookingDate != null) yield return BookingDate;
            yield return Url;
            yield return Details;
        }
    }
}

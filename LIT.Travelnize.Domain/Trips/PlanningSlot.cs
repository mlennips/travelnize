
namespace LIT.Travelnize.Domain.Trips
{
    public record PlanningSlot : ValueObject
    {
        public int Order { get; }
        public DateRange? DateRange { get; }

        public PlanningSlot(int order, DateRange? dateRange = null)
        {
            Order = order;
            DateRange = dateRange;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Order;
            if (DateRange != null) yield return DateRange;
        }

        public static PlanningSlot Create(int order, DateTime start, DateTime? end)
        {
            var dateRange = new DateRange(start, end);
            return new PlanningSlot(order, dateRange);
        }
    }
}

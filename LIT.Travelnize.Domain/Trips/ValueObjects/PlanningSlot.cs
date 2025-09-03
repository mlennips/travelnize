namespace LIT.Travelnize.Domain.Trips.ValueObjects
{
    public record PlanningSlot : ValueObject
    {
        public int Order { get; init; }
        public DateTime? Start { get; init; }
        public DateTime? End { get; init; }

        private PlanningSlot() { } // For EF Core

        public PlanningSlot(int order, DateTime? start, DateTime? end)
        {
            if (end <= start && end != null)
            {
                throw new ArgumentException("End date must be after start date.");
            }
            Order = order;
            Start = start;
            End = end;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Order;
            if (Start != null) yield return Start;
            if (End != null) yield return End;
        }

        public static PlanningSlot Create(int order, DateTime? start, DateTime? end)
        {
            return new PlanningSlot(order, start, end);
        }
    }
}

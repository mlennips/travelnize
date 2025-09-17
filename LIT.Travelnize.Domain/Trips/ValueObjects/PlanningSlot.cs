namespace LIT.Travelnize.Domain.Trips.ValueObjects
{
    public record PlanningSlot : ValueObject, IComparable<PlanningSlot>
    {
        public DateTime? Start { get; init; }
        public DateTime? End { get; init; }

        private PlanningSlot() { } // For EF Core

        public PlanningSlot(DateTime? start, DateTime? end)
        {
            if (end <= start && end != null)
            {
                throw new ArgumentException("End date must be after start date.");
            }
            Start = start;
            End = end;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            if (Start != null) yield return Start;
            if (End != null) yield return End;
        }

        public static PlanningSlot Create(DateTime? start, DateTime? end)
        {
            return new PlanningSlot(start, end);
        }

        public int CompareTo(PlanningSlot? other)
        {
            if (other is null) return 1;
            if (Start == null && other.Start == null) return 0;
            if (Start == null) return -1;
            if (other.Start == null) return 1;
            return Start.Value.CompareTo(other.Start.Value);
        }

        public static bool operator <(PlanningSlot? left, PlanningSlot? right)
        {
            if (left is null) return right is not null;
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(PlanningSlot? left, PlanningSlot? right)
        {
            if (right is null) return left is not null;
            return left?.CompareTo(right) > 0;
        }

        public static bool operator <=(PlanningSlot? left, PlanningSlot? right)
        {
            if (left is null) return true;
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(PlanningSlot? left, PlanningSlot? right)
        {
            if (right is null) return true;
            return left?.CompareTo(right) >= 0;
        }
    }
}

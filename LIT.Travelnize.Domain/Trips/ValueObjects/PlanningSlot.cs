using System.Globalization;

namespace LIT.Travelnize.Domain.Trips.ValueObjects
{
    public record PlanningSlot : ValueObject, IComparable<PlanningSlot>
    {
        public DateTime? Start { get; init; }
        public DateTime? End { get; init; }

        public int TotalDays =>
            Start.HasValue && End.HasValue
                ? (End.Value.Date - Start.Value.Date).Days
                : 0;

        public int TotalDaysInclusive =>
            Start.HasValue && End.HasValue
                ? (End.Value.Date - Start.Value.Date).Days + 1
                : 0;

        public int Nights =>
            Start.HasValue && End.HasValue
                ? Math.Max(0, (End.Value.Date - Start.Value.Date).Days)
                : 0;

        public int RemainingDays
        {
            get
            {
                if (End == null) return 0;
                var days = (End.Value.Date - DateTime.Now.Date).Days;
                return days > 0 ? days : 0;
            }
        }

        public int ActiveDays
        {
            get
            {
                if (Start == null) return 0;
                var today = DateTime.Now.Date;
                if (today < Start.Value.Date) return 0;
                var end = End?.Date ?? today;
                var days = (today - Start.Value.Date).Days;
                var maxDays = (end - Start.Value.Date).Days;
                return Math.Min(days, maxDays > 0 ? maxDays : 0) + 1;
            }
        }

        public int DaysUntilStart
        {
            get
            {
                if (Start == null) return 0;
                var days = (Start.Value.Date - DateTime.Now.Date).Days;
                return days > 0 ? days : 0;
            }
        }

        private PlanningSlot() { }

        public PlanningSlot(DateTime? start, DateTime? end)
        {
            if (end <= start && end != null)
            {
                throw new ArgumentException("End date must be after start date.");
            }
            Start = start;
            End = end;
        }

        public static PlanningSlot Create(DateTime? start, DateTime? end)
        {
            return new PlanningSlot(start, end);
        }

        public static PlanningSlot Empty => new(null, null);

        public string ToShortDateString()
        {
            if (Start.HasValue && End.HasValue && Start.Value.Year == End.Value.Year)
            {
                var culture = CultureInfo.CurrentCulture;
                var pattern = culture.DateTimeFormat.ShortDatePattern;

                // Jahr-Anteil entfernen (alle y-Gruppen) und abschließende Trenner säubern
                var patternWithoutYear = System.Text.RegularExpressions.Regex
                    .Replace(pattern, "y+", "")
                    .TrimEnd('.', '-', '/', ',', ' ')
                    .Trim();

                if (string.IsNullOrWhiteSpace(patternWithoutYear))
                    return Start.Value.ToShortDateString() + " - " + End.Value.ToShortDateString();

                var startStr = Start.Value.ToString(patternWithoutYear, culture);
                var endStr = End.Value.ToString(pattern, culture);
                return startStr + " - " + endStr;
            }

            return (Start?.ToShortDateString() ?? string.Empty) + " - " + (End?.ToShortDateString() ?? string.Empty);
        }

        public string ToLongDateString() => Start?.ToLongDateString() + " - " + End?.ToLongDateString();

        public int CompareTo(PlanningSlot? other)
        {
            if (other is null) return 1;
            if (Start == null && other.Start == null) return 0;
            if (Start == null) return -1;
            if (other.Start == null) return 1;
            return Start.Value.CompareTo(other.Start.Value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            if (Start != null) yield return Start;
            if (End != null) yield return End;
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

namespace LIT.Travelnize.Domain.Trips.ValueObjects
{
    public record PlanningSlot : ValueObject, IComparable<PlanningSlot>
    {
        public DateTime? Start { get; init; }
        public DateTime? End { get; init; }
        public int TotalDays => (int)((End ?? DateTime.MinValue) - (Start ?? DateTime.MinValue)).TotalDays;

        /// <summary>
        /// Gibt die verbleibenden Tage bis zum Ende des Slots zurück (ab heute).
        /// Ist das Enddatum in der Vergangenheit oder nicht gesetzt, wird 0 zurückgegeben.
        /// </summary>
        public int RemainingDays
        {
            get
            {
                if (End == null) return 0;
                var days = (End.Value.Date - DateTime.Now.Date).Days;
                return days > 0 ? days : 0;
            }
        }

        /// <summary>
        /// Gibt die Anzahl der bereits aktiven Tage zurück (seit Start bis heute).
        /// Ist der Slot noch nicht gestartet oder das Startdatum nicht gesetzt, wird 0 zurückgegeben.
        /// </summary>
        public int ActiveDays
        {
            get
            {
                if (Start == null) return 0;
                var today = DateTime.Now.Date;
                if (today < Start.Value.Date) return 0;
                var end = End?.Date ?? today;
                var days = (today - Start.Value.Date).Days;
                // Begrenzung auf die Gesamtdauer des Slots
                var maxDays = (end - Start.Value.Date).Days;
                return Math.Min(days, maxDays > 0 ? maxDays : 0) + 1;
            }
        }

        /// <summary>
        /// Gibt die Anzahl der Tage bis zum Start des Slots zurück (ab heute).
        /// Ist das Startdatum in der Vergangenheit oder nicht gesetzt, wird 0 zurückgegeben.
        /// </summary>
        public int DaysUntilStart
        {
            get
            {
                if (Start == null) return 0;
                var days = (Start.Value.Date - DateTime.Now.Date).Days;
                return days > 0 ? days : 0;
            }
        }

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

        public static PlanningSlot Create(DateTime? start, DateTime? end)
        {
            return new PlanningSlot(start, end);
        }

        public string ToShortDateString() => Start?.ToShortDateString() + " - " + End?.ToShortDateString();
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

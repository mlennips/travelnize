namespace LIT.Travelnize.Domain.Common
{
    public record DateRange : ValueObject
    {
        public DateTime Start { get; init; }
        public DateTime? End { get; init; }

        public DateRange(DateTime start, DateTime? end)
        {
            if (end <= start && end != null)
            {
                throw new ArgumentException("End date must be after start date.");
            }

            Start = start;
            End = end;
        }

        public override string ToString() =>
            End == null
                ? $"{Start.ToShortDateString()} - ..."
                : $"{Start.ToShortDateString()} - {End.Value.ToShortDateString()}";

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            if (End != null) yield return End;
        }
    }
}
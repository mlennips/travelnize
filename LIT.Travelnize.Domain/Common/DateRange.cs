using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Common
{
    public record DateRange : ValueObject
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        public DateRange(DateTime start, DateTime end)
        {
            if (end <= start)
            {
                throw new ArgumentException("End date must be after start date.");
            }

            Start = start;
            End = end;
        }

        public bool Overlaps(DateRange other)
        {
            return Start < other.End && End > other.Start;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }
    }
}
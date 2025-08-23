namespace LIT.Travelnize.Domain.Base
{
    public abstract record SingleValueObject<T> : ValueObject, IComparable<SingleValueObject<T>>
    {
        public T Value { get; init; }

        public SingleValueObject(T value)
        {
            if (value is null)
            {
                throw new ArgumentException("Wert darf nicht NULL sein.");
            }
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value!.GetHashCode();
        }

        public virtual int CompareTo(SingleValueObject<T>? other)
        {
            return other is not null
            ? string.Compare(ToString(), other.ToString(), StringComparison.Ordinal) : -1;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }

        public static bool operator <(SingleValueObject<T> left, SingleValueObject<T> right)
        {
            return left is null ? right is not null : left.CompareTo(right) < 0;
        }

        public static bool operator <=(SingleValueObject<T> left, SingleValueObject<T> right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(SingleValueObject<T> left, SingleValueObject<T> right)
        {
            return left is not null && left.CompareTo(right) > 0;
        }

        public static bool operator >=(SingleValueObject<T> left, SingleValueObject<T> right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            if (Value is null)
            {
                yield break;
            }
            yield return Value;
        }
    }
}

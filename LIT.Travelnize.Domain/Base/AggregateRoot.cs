namespace LIT.Travelnize.Domain.Base
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        public abstract Guid Id { get; init; }
        public Guid AggregateId => Id;

        public virtual Result Delete()
        {
            return Result.Success();
        }
    }
}

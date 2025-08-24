
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Domain.Base
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        public abstract Guid Id { get; init; }

        protected virtual Result Delete()
        {
            return Result.Success();
        }
    }
}

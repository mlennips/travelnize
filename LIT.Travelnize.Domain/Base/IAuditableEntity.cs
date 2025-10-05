namespace LIT.Travelnize.Domain.Base
{
    public  interface IAuditableEntity : IEntity
    {
        Guid AggregateId { get; }
    }
}

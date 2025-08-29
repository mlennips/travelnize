using System.Linq.Expressions;

namespace LIT.Travelnize.Domain.Base
{
    public interface ISpecification<TEntity>
        where TEntity : class, IEntity
    {
        Expression<Func<TEntity, bool>> Criteria { get; }
        IQueryable<TEntity> FinalProcessing(IQueryable<TEntity> filteredQueryable);
    }
}

using System.Linq.Expressions;

namespace LIT.Travelnize.Domain.Base
{
    public interface ISpecification<TEntity>
        where TEntity : class, IEntity
    {
        Expression<Func<TEntity, bool>> Criteria { get; }
        //List<Expression<Func<TEntity, object>>> Includes { get; } // ToDo noch nötig?
        IQueryable<TEntity> FinalProcessing(IQueryable<TEntity> filteredQueryable);
    }
}

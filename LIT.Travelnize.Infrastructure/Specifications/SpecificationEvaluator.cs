using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Infrastructure.Specifications
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> ApplySpecification<TEntity>(
            IQueryable<TEntity> inputQueryable,
            ISpecification<TEntity> specification)
            where TEntity : class, IEntity
        {
            var filteredQueryable = inputQueryable.Where(specification.Criteria);
            filteredQueryable = specification.FinalProcessing(filteredQueryable);
            return filteredQueryable;
        }
    }
}

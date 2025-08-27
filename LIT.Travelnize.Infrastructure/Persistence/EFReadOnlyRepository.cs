using LIT.Travelnize.Domain.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    internal sealed class EFReadOnlyRepository<T>(AppDbContext appDbContext) : IReadOnlyRepository<T>
        where T : class, IEntity
    {
        public async Task<ReadOnlyCollection<T>> GetAllAsync()
        {
            var items = await appDbContext.Set<T>()
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync();
            return items.AsReadOnly();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            var item = await appDbContext.Set<T>().FindAsync(id);
            if (item != null)
            {
                appDbContext.Entry(item).State = EntityState.Detached;
            }
            return item;
        }

        public async Task<IReadOnlyCollection<TResult>> FindByAsync<TResult>(ISpecification<T> specification, Func<T, TResult> map)
        {
            var allItemsQueryable = appDbContext.Set<T>().AsQueryable();
            var filteredItemsQueryable = Specifications.SpecificationEvaluator.ApplySpecification(allItemsQueryable, specification);            
            var mappedItemsQueryable = filteredItemsQueryable.Select(map).AsQueryable();
            var result = await mappedItemsQueryable.ToListAsync();
            return result;
        }
    }
}

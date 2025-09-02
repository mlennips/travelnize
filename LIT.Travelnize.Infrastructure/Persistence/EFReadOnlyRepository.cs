using LIT.Travelnize.Domain.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    internal sealed class EFReadOnlyRepository<T>(AppDbContext appDbContext) : IReadOnlyRepository<T>
        where T : class, IEntity
    {
        public async Task<T?> GetByIdAsync(Guid id)
        {
            var item = await appDbContext.Set<T>().FindAsync(id);
            if (item != null)
            {
                appDbContext.Entry(item).State = EntityState.Detached;
            }
            return item;
        }

        public async Task<T[]> GetAllAsync()
        {
            var items = await appDbContext.Set<T>()
                .AsNoTrackingWithIdentityResolution()
                .ToArrayAsync();
            return items;
        }

        public async Task<TResult[]> FindByAsync<TResult>(ISpecification<T> specification, Func<T, TResult> map)
        {
            var allItemsQueryable = appDbContext.Set<T>().AsQueryable();
            var filteredItems = await Specifications.SpecificationEvaluator.ApplySpecification(allItemsQueryable, specification).ToListAsync();
            var mappedItems = filteredItems.Select(map).ToArray();
            return mappedItems;
        }
    }
}

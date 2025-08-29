using LIT.Travelnize.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace LIT.Travelnize.Infrastructure.Persistence
{
    internal sealed class UnitOfWork(AppDbContext appDbContext, ICurrentUser currentUser) : IUnitOfWork
    {
        public bool HasChanges => appDbContext.ChangeTracker.HasChanges();

        public async Task<IUser> GetUserAsync()
        {
            return await currentUser.GetUserAsync();
        }

        public async Task<T?> GetByIdAsync<T>(Guid id) where T : class, IAggregateRoot
        {
            return await appDbContext.Set<T>().FindAsync(id);
        }

        public async Task AddAsync<T>(T item) where T : class, IAggregateRoot
        {
            await appDbContext.Set<T>().AddAsync(item);
        }
        public Task UpdateAsync<T>(T item) where T : class, IAggregateRoot
        {
            return Task.CompletedTask;
        }

        public Task RemoveAsync<T>(T item) where T : class, IAggregateRoot
        {
            appDbContext.Set<T>().Remove(item);
            return Task.CompletedTask;
        }

        public async Task RemoveAsync<T>(Guid id) where T : class, IAggregateRoot
        {
            var entity = await appDbContext.Set<T>().FindAsync(id);
            if (entity != null)
            {
                appDbContext.Set<T>().Remove(entity);
            }
        }

        public Task CommitAsync(CancellationToken? cancellationToken = null)
        {
            return appDbContext.SaveChangesAsync(cancellationToken ?? CancellationToken.None);
        }

        public void Dispose()
        {
            appDbContext.Dispose();
        }
    }
}

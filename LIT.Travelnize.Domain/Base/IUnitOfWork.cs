namespace LIT.Travelnize.Domain.Base
{
    public interface IUnitOfWork : IDisposable
    {
        bool HasChanges { get; }

        Task AddAsync<T>(T item) where T : class, IAggregateRoot;
        Task RemoveAsync<T>(T item) where T : class, IAggregateRoot;
        Task RemoveAsync<T>(Guid id) where T : class, IAggregateRoot;
        Task<T?> GetByIdAsync<T>(Guid id) where T : class, IAggregateRoot;
        Task CommitAsync(CancellationToken? cancellationToken = null);
        Task<IUser> GetUserAsync();
    }
}

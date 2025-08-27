using LIT.Travelnize.Domain.Common;
using System.Collections.ObjectModel;

namespace LIT.Travelnize.Domain.Base
{
    public interface IReadOnlyRepository<T> where T : class, IEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<ReadOnlyCollection<T>> GetAllAsync();
        Task<IReadOnlyCollection<TResult>> FindByAsync<TResult>(ISpecification<T> specification, Func<T, TResult> map);
    }
}

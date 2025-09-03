using MediatR;

namespace LIT.Travelnize.Domain.Base
{
    public interface IQuery<T> : IRequest<Result<T>>
    {
    }
}

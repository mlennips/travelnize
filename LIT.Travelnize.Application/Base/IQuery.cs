using LIT.Travelnize.Domain.Common;
using MediatR;

namespace LIT.Travelnize.UseCases.Base
{
    public interface IQuery<T> : IRequest<Result<T>>
    {
    }
}

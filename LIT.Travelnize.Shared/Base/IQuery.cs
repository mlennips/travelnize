using LIT.Travelnize.Shared.Common;
using MediatR;

namespace LIT.Travelnize.Shared.Base
{
    public interface IQuery<T> : IRequest<Result<T>>
    {
    }
}

using LIT.Travelnize.Shared.Common;
using MediatR;

namespace LIT.Travelnize.Shared.Base
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
    {
    }
}

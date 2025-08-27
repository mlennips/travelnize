using LIT.Travelnize.Domain.Common;
using MediatR;

namespace LIT.Travelnize.UseCases.Base
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
    {
    }
}

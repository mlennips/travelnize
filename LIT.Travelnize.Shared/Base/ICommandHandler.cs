using LIT.Travelnize.Shared.Common;
using MediatR;

namespace LIT.Travelnize.Shared.Base
{
    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
        where TCommand : ICommand<TResponse>
        where TResponse : notnull
    {

    }

    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
        where TCommand : IRequest<Result>
    {

    }
}

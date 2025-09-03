using MediatR;

namespace LIT.Travelnize.Domain.Base
{
    public interface ICommand<T> : ICommandBase, IRequest<Result<T>>
        where T : notnull
    {

    }

    public interface ICommand : ICommandBase, IRequest<Result>
    {

    }

    public interface ICommandBase : IRequest
    {

    }
}

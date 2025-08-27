using LIT.Travelnize.Domain.Common;
using MediatR;

namespace LIT.Travelnize.UseCases.Base
{
    public interface ICommand<T> : Domain.Base.ICommand, IRequest<Result<T>>
        where T : notnull
    {

    }

    public interface ICommand : Domain.Base.ICommand, IRequest<Result>
    {

    }
}

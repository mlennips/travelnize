using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;
using MediatR;

namespace LIT.Travelnize.Infrastructure.Messaging
{
    public class UnitOfWorkBehavior<TRequest, TResponse>(IUnitOfWork unitOfWork)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommandBase
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next(cancellationToken);
            var hasChanges = unitOfWork.HasChanges;
            if (hasChanges)
            {
                if (response is Result result && result.IsSuccess)
                {
                    // If the response is a Result and indicates success, we commit the changes.
                    await unitOfWork.CommitAsync(cancellationToken);
                }
                else if (response is Result resultWithError && resultWithError.IsFailure)
                {
                    // If the response is a Result with an error, we throw an exception
                    // to indicate that the commit failed due to an unsuccessful operation.
                    throw new InvalidOperationException($"Unit of Work commit failed due to an unsuccessful result: {resultWithError.Error.Description}");
                }
                else
                {
                    // If the response is not a Result, we assume it is a command that has been handled successfully.
                    // We can commit the changes without checking for success.
                    await unitOfWork.CommitAsync(cancellationToken);
                }
            }
            return response;
        }
    }
}

using LIT.Travelnize.Domain.Common;
using MediatR;

namespace LIT.Travelnize.API.Utils
{
    public static class ApiServiceExtensions
    {
        public static async Task<IResult> SendAndMatchAsync<TResult>(this IMediator mediator, IRequest<Result<TResult>> request, Func<TResult, IResult> onSuccess, Func<ErrorDetail, IResult> onFailure)
            where TResult : notnull
        {
            Result<TResult> response = await mediator.Send(request!);
            return response is Result result
                ? result.IsSuccess
                    ? result.Value is TResult value ? onSuccess(value) : throw new InvalidOperationException("Wrong value type.")
                    : onFailure(result.Error)
                : throw new InvalidOperationException("Wrong response type.");
        }

        public static async Task<IResult> SendAndMatchAsync(this IMediator mediator, IBaseRequest request, Func<IResult>? onSuccess = null, Func<ErrorDetail, IResult>? onFailure = null)
        {
            onSuccess ??= () => Results.Ok();
            onFailure ??= Results.BadRequest;
            var response = await mediator.Send(request!);
            return response is Result result
                ? result.IsSuccess ? onSuccess() : onFailure(result.Error)
                : throw new InvalidOperationException("Wrong response type.");
        }
    }
}

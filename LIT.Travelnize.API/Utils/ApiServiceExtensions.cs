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
            if (response is Result result)
            {
                if (result.IsSuccess)
                {
                    if (result.Value is TResult value)
                    {
                        return onSuccess(value);
                    }
                    else
                    {
                        throw new InvalidOperationException("Wrong value type.");
                    }
                }
                else
                {
                    return onFailure(result.Error);
                }
            }
            else
            {
                throw new InvalidOperationException("Wrong response type.");
            }
        }

        public static async Task<IResult> SendAndMatchAsync(this IMediator mediator, IBaseRequest request, Func<IResult>? onSuccess = null, Func<ErrorDetail, IResult>? onFailure = null)
        {
            onSuccess ??= () => Results.Ok();
            onFailure ??= Results.BadRequest;
            var response = await mediator.Send(request!);

            if (response is Result result)
            {
                if (result.IsSuccess)
                {
                    return onSuccess();
                }
                else
                {
                    return onFailure(result.Error);
                }
            }
            else
            {
                throw new InvalidOperationException("Wrong response type.");
            }
        }
    }
}

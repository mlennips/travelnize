using LIT.Travelnize.API.Utils;
using LIT.Travelnize.Shared.Common;
using LIT.Travelnize.Shared.Trips;
using MediatR;

namespace LIT.Travelnize.API.Endpoints
{
    public static class Trips
    {
        public static void RegisterUsersEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder api = routes.MapGroup("/trips")
                .WithTags("Trips")
                .RequireAuthorization();

            api.MapPost("/", async (IMediator mediator, CreateTripCommand command) =>
                await mediator.SendAndMatchAsync(command,
                    onSuccess: Results.Ok,
                    onFailure: Results.BadRequest))
                .Produces<Guid>(StatusCodes.Status200OK);

            api.MapGet("/", async (IMediator mediator, Guid userId) =>
                await mediator.SendAndMatchAsync(new ListTripsQuery(userId),
                    onSuccess: Results.Ok,
                    onFailure: Results.BadRequest))
                .Produces<ListTripsDto[]>(StatusCodes.Status200OK);

            api.MapGet("/{tripId}", async (IMediator mediator, Guid tripId) =>
                await mediator.SendAndMatchAsync(new GetTripQuery(tripId),
                    onSuccess: Results.Ok,
                    onFailure: Results.BadRequest))
                .Produces<GetTripDto>(StatusCodes.Status200OK);

            api.MapPut("/{tripId}", async (IMediator mediator, Guid tripId, UpdateTripCommand command) =>
                await mediator.SendAndMatchAsync(command,
                    onSuccess: Results.NoContent,
                    onFailure: Results.BadRequest))
                .Produces(StatusCodes.Status204NoContent);

            api.MapDelete("/{tripId}", async (IMediator mediator, Guid tripId) =>
                await mediator.SendAndMatchAsync(new DeleteTripCommand(tripId),
                    onSuccess: () => Results.NoContent(),
                    onFailure: Results.BadRequest))
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);
        }
    }
}

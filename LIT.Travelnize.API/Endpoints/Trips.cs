using LIT.Travelnize.API.Utils;
using MediatR;
using static LIT.Travelnize.UseCases.Trips.CreateTrip;
using static LIT.Travelnize.UseCases.Trips.GetTrip;

namespace LIT.Travelnize.API.Endpoints
{
    public static class Trips
    {
        public static void RegisterUsersEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder api = routes.MapGroup("/trips")
                .WithTags(["Trips"])
                .RequireAuthorization();

            api.MapPost("/", async (IMediator mediator, CreateTripCommand command) =>
                await mediator.SendAndMatchAsync(command,
                    onSuccess: Results.Ok,
                    onFailure: Results.BadRequest))
                .Produces<Guid>(StatusCodes.Status200OK);
            //api.MapGet("/", async (IMediator mediator) =>
            //    await mediator.SendAndMatchAsync(new ListTripsQuery(),
            //        onSuccess: Results.Ok,
            //        onFailure: Results.BadRequest))
            //    .Produces<ListTripDTO[]>(StatusCodes.Status200OK);

            api.MapGet("/{tripId}", async (IMediator mediator, Guid tripId) =>
                await mediator.SendAndMatchAsync(new GetTripQuery(tripId),
                    onSuccess: Results.Ok,
                    onFailure: Results.BadRequest))
                .Produces<GetTripDTO>(StatusCodes.Status200OK);

            //api.MapPut("/{tripId}", async (IMediator mediator, Guid tripId, UpdateTripCommand command) =>
            //    await mediator.SendAndMatchAsync(command,
            //        onSuccess: Results.NoContent,
            //        onFailure: Results.BadRequest))
            //    .Produces(StatusCodes.Status204NoContent);

            //api.MapDelete("/{tripId}", async (IMediator mediator, Guid tripId) =>
            //    await mediator.SendAndMatchAsync(new DeleteCustomerCommand(tripId),
            //        onSuccess: () => Results.NoContent(),
            //        onFailure: Results.BadRequest))
            //    .Produces(StatusCodes.Status204NoContent)
            //    .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);
        }
    }
}

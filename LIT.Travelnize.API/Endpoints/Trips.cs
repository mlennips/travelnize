using LIT.Travelnize.API.Utils;
using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.Queries;
using MediatR;

namespace LIT.Travelnize.API.Endpoints
{
    public static class Trips
    {
        public static void MapTripsEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder api = routes.MapGroup("/trips")
                .WithTags("Trips")
                .RequireAuthorization();

            MapTrips(api);
            MapTravelSegments(api);
            MapDestinations(api);
            MapParticipants(api);
            MapTransportation(api);
            MapAccommodation(api);
            MapActivities(api);
        }

        private static void MapAccommodation(RouteGroupBuilder api)
        {
            api.MapPost("/{tripId}/destinations/{destinationId}/accommodations", async (IMediator mediator, Guid tripId, Guid destinationId, AddAccommodationCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.Ok,
                        onFailure: Results.BadRequest))
                .Produces<Guid>(StatusCodes.Status200OK)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);
        }

        private static void MapTrips(RouteGroupBuilder api)
        {
            // Trip CRUD
            api.MapPost("/", async (IMediator mediator, CreateTripCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.Ok,
                        onFailure: Results.BadRequest))
                .Produces<Guid>(StatusCodes.Status200OK)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapGet("/", async (IMediator mediator, Guid userId) =>
                    await mediator.SendAndMatchAsync(new ListTripsQuery(userId),
                        onSuccess: Results.Ok,
                        onFailure: Results.BadRequest))
                .Produces<ListTripsResponse[]>(StatusCodes.Status200OK)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapGet("/{tripId}", async (IMediator mediator, Guid tripId) =>
                    await mediator.SendAndMatchAsync(new GetTripQuery(tripId),
                        onSuccess: Results.Ok,
                        onFailure: Results.BadRequest))
                .Produces<GetTripResponse>(StatusCodes.Status200OK)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapPut("/{tripId}", async (IMediator mediator, Guid tripId, UpdateTripCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.NoContent,
                        onFailure: Results.BadRequest))
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapDelete("/{tripId}", async (IMediator mediator, Guid tripId) =>
                    await mediator.SendAndMatchAsync(new DeleteTripCommand(tripId),
                        onSuccess: () => Results.NoContent(),
                        onFailure: Results.BadRequest))
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);
        }

        private static void MapTravelSegments(RouteGroupBuilder api)
        {
            api.MapPost("/{tripId}/segments", async (IMediator mediator, Guid tripId, AddTravelSegmentCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.Ok,
                        onFailure: Results.BadRequest))
                .Produces<Guid>(StatusCodes.Status200OK)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapPut("/{tripId}/segments/{segmentId}", async (IMediator mediator, Guid tripId, Guid segmentId, UpdateTravelSegmentCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.NoContent,
                        onFailure: Results.BadRequest))
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapDelete("/{tripId}/segments/{segmentId}", async (IMediator mediator, Guid tripId, Guid segmentId) =>
                    await mediator.SendAndMatchAsync(new RemoveTravelSegmentCommand(tripId, segmentId),
                        onSuccess: () => Results.NoContent(),
                        onFailure: Results.BadRequest))
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);
        }

        private static void MapDestinations(RouteGroupBuilder api)
        {
            api.MapPost("/{tripId}/segments/{segmentId}/destinations", async (IMediator mediator, Guid tripId, Guid segmentId, AddDestinationCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.Ok,
                        onFailure: Results.BadRequest))
                .Produces<Guid>(StatusCodes.Status200OK)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapPut("/{tripId}/segments/{segmentId}/destinations/{destinationId}", async (IMediator mediator, Guid tripId, Guid segmentId, Guid destinationId, UpdateDestinationCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.NoContent,
                        onFailure: Results.BadRequest))
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapDelete("/{tripId}/segments/{segmentId}/destinations/{destinationId}", async (IMediator mediator, Guid tripId, Guid segmentId, Guid destinationId) =>
                    await mediator.SendAndMatchAsync(new RemoveDestinationCommand(tripId, segmentId, destinationId),
                        onSuccess: () => Results.NoContent(),
                        onFailure: Results.BadRequest))
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);
        }

        private static void MapParticipants(RouteGroupBuilder api)
        {
            api.MapPost("/{tripId}/participants", async (IMediator mediator, Guid tripId, AddParticipantCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.Ok,
                        onFailure: Results.BadRequest))
                .Produces<Guid>(StatusCodes.Status200OK)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapPost("/{tripId}/participants/guest", async (IMediator mediator, Guid tripId, AddGuestParticipantCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.Ok,
                        onFailure: Results.BadRequest))
                .Produces<Guid>(StatusCodes.Status200OK)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapPut("/{tripId}/participants/{participantId}", async (IMediator mediator, Guid tripId, Guid participantId, UpdateParticipantCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.NoContent,
                        onFailure: Results.BadRequest))
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapPut("/{tripId}/participants/{participantId}/permission", async (IMediator mediator, Guid tripId, Guid participantId, ChangeParticipantPermissionCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.NoContent,
                        onFailure: Results.BadRequest))
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);

            api.MapDelete("/{tripId}/participants/{participantId}", async (IMediator mediator, Guid tripId, Guid participantId) =>
                    await mediator.SendAndMatchAsync(new RemoveParticipantCommand(tripId, participantId),
                        onSuccess: () => Results.NoContent(),
                        onFailure: Results.BadRequest))
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);
        }

        private static void MapTransportation(RouteGroupBuilder api)
        {
            api.MapPost("/{tripId}/transportations", async (IMediator mediator, Guid tripId, AddTransportationCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.Ok,
                        onFailure: Results.BadRequest))
                .Produces<Guid>(StatusCodes.Status200OK)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);
        }

        private static void MapActivities(RouteGroupBuilder api)
        {
            api.MapPost("/{tripId}/destinations/{destinationId}/activities", async (IMediator mediator, Guid tripId, Guid destinationId, AddActivityCommand command) =>
                    await mediator.SendAndMatchAsync(command,
                        onSuccess: Results.Ok,
                        onFailure: Results.BadRequest))
                .Produces<Guid>(StatusCodes.Status200OK)
                .Produces<ErrorDetail>(StatusCodes.Status400BadRequest);
        }
    }
}

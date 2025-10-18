using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Destinations
{
    public class RemoveDestinationHandler(IUnitOfWork uow) : ICommandHandler<RemoveDestinationCommand>
    {
        public async Task<Result> Handle(RemoveDestinationCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.RemoveDestinationFromTravelSegment(request.SegmentId, request.DestinationId);
            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }
            return result;
        }
    }
}
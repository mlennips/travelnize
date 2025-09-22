using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Trips
{
    public class UpdateDestinationHandler(IUnitOfWork uow) : ICommandHandler<UpdateDestinationCommand>
    {
        public async Task<Result> Handle(UpdateDestinationCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.UpdateDestinationInTravelSegment(request.SegmentId, request.DestinationId,
                request.Name, request.Description, request.Location);

            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }
            return result;
        }
    }
}
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Trips
{
    public class RemoveTravelSegmentHandler(IUnitOfWork uow) : ICommandHandler<RemoveTravelSegmentCommand>
    {
        public async Task<Result> Handle(RemoveTravelSegmentCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.RemoveTravelSegment(request.SegmentId);
            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }
            return result;
        }
    }
}
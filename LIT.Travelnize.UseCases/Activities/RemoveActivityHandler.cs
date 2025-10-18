using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Activities
{
    public class RemoveActivityHandler(IUnitOfWork uow) : ICommandHandler<RemoveActivityCommand>
    {
        public async Task<Result> Handle(RemoveActivityCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.RemoveActivity(request.DestinationId, request.ActivityId);
            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }
            return result;
        }
    }
}
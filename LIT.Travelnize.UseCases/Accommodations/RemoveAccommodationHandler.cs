using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Accommodations
{
    public class RemoveAccommodationHandler(IUnitOfWork uow) : ICommandHandler<RemoveAccommodationCommand>
    {
        public async Task<Result> Handle(RemoveAccommodationCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.RemoveAccommodation(request.DestinationId, request.AccommodationId);
            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }
            return result;
        }
    }
}
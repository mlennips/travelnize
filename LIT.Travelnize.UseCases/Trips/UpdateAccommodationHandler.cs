using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Trips
{
    public class UpdateAccommodationHandler(IUnitOfWork uow) : ICommandHandler<UpdateAccommodationCommand>
    {
        public async Task<Result> Handle(UpdateAccommodationCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.UpdateAccommodation(
                request.DestinationId,
                request.AccommodationId,
                request.Name,
                request.Type,
                request.Address,
                request.CheckInOut,
                request.BookingInfo);

            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }

            return result;
        }
    }
}
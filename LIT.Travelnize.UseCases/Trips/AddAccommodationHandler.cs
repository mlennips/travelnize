using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Trips
{
    public class AddAccommodationHandler(IUnitOfWork uow) : ICommandHandler<AddAccommodationCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AddAccommodationCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.AddAccommodationToDestination(request.DestinationId, request.Name,
                request.Type, request.Address, request.CheckInOut);

            if (!result.IsSuccess) return result.Error;

            await uow.UpdateAsync(trip);
            return result.Value!.Id;
        }
    }
}
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Trips
{
    public class DeleteTripHandler(IUnitOfWork uow) : ICommandHandler<DeleteTripCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(DeleteTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.Delete();
            if (!result.IsSuccess) return result.Error;

            await uow.RemoveAsync(trip);
            return trip.Id;
        }
    }
}
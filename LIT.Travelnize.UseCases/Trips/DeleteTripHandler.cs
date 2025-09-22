using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Trips
{
    public class DeleteTripHandler(IUnitOfWork uow) : ICommandHandler<DeleteTripCommand>
    {
        public async Task<Result> Handle(DeleteTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip == null)
            {
                return TripErrors.TripNotFound;
            }
            var result = trip.Delete();
            await uow.RemoveAsync(trip);
            return result;
        }
    }
}
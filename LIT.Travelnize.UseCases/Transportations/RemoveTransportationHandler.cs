using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Transportations
{
    public class RemoveTransportationHandler(IUnitOfWork uow) : ICommandHandler<RemoveTransportationCommand>
    {
        public async Task<Result> Handle(RemoveTransportationCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.RemoveTransportation(request.TransportationId);
            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }
            return result;
        }
    }
}
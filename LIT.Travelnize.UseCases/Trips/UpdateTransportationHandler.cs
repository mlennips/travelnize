using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Trips
{
    public class UpdateTransportationHandler(IUnitOfWork uow) : ICommandHandler<UpdateTransportationCommand>
    {
        public async Task<Result> Handle(UpdateTransportationCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.UpdateTransportation(
                request.TransportationId,
                request.Name,
                request.Description,
                request.Identifier,
                request.Departure,
                request.Arrival,
                request.DepartureDate,
                request.ArrivalDate,
                request.RouteWebsite,
                request.Type,
                request.PassengerIds);

            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }
            return result;
        }
    }
}
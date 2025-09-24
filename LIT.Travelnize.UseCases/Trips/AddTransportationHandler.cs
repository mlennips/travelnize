using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Trips
{
    public class AddTransportationHandler(IUnitOfWork uow) : ICommandHandler<AddTransportationCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AddTransportationCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.AddTransportation(request.Name, request.Description, request.Identifier,
                request.Departure, request.Arrival, request.DepartureDate, request.ArrivalDate,
                request.RouteWebsite, request.Type);

            if (!result.IsSuccess) return result.Error;

            await uow.UpdateAsync(trip);
            return result.Value!.Id;
        }
    }
}
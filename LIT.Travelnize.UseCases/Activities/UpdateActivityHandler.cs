using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Activities
{
    public class UpdateActivityHandler(IUnitOfWork uow) : ICommandHandler<UpdateActivityCommand>
    {
        public async Task<Result> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.UpdateActivity(request.DestinationId, request.ActivityId, request.Name,
                request.Description, request.Location, request.Date, request.Duration);

            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }
            return result;
        }
    }
}

using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Trips
{
    public class AddActivityHandler(IUnitOfWork uow) : ICommandHandler<AddActivityCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AddActivityCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.AddActivity(request.DestinationId, request.Name, request.Description,
                request.Location, request.Date, request.Duration);

            if (!result.IsSuccess) return result.Error;

            await uow.UpdateAsync(trip);
            return result.Value!.Id;
        }
    }
}
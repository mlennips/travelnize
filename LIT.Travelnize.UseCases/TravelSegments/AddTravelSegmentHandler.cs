using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.TravelSegments
{
    public class AddTravelSegmentHandler(IUnitOfWork uow) : ICommandHandler<AddTravelSegmentCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AddTravelSegmentCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.AddTravelSegment(request.Name, request.Description, request.Slot);

            if (!result.IsSuccess) return result.Error;

            await uow.UpdateAsync(trip);
            return result.Value!.Id;
        }
    }
}
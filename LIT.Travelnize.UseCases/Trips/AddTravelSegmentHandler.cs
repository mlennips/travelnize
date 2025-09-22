using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.UseCases.Trips
{
    public class AddTravelSegmentHandler(IUnitOfWork uow) : ICommandHandler<AddTravelSegmentCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AddTravelSegmentCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var slot = PlanningSlot.Create(request.Start, request.End);
            var result = trip.AddTravelSegment(slot, request.Description);

            if (!result.IsSuccess) return result.Error;

            await uow.UpdateAsync(trip);
            return result.Value!.Id;
        }
    }
}
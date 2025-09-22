using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.UseCases.Trips
{
    public class AddDestinationHandler(IUnitOfWork uow) : ICommandHandler<AddDestinationCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AddDestinationCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            PlanningSlot? slot = PlanningSlot.Create(request.Start, request.End);

            var result = trip.AddDestinationToTravelSegment(request.SegmentId, request.Name, request.Description, request.Location, slot);
            if (!result.IsSuccess) return result.Error;

            await uow.UpdateAsync(trip);
            return result.Value!.Id;
        }
    }
}
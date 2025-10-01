using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.ValueObjects;

namespace LIT.Travelnize.UseCases.Trips
{
    public class UpdateTravelSegmentHandler(IUnitOfWork uow) : ICommandHandler<UpdateTravelSegmentCommand>
    {
        public async Task<Result> Handle(UpdateTravelSegmentCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip == null)
            {
                return TripErrors.TripNotFound;
            }

            var result = trip.UpdateTravelSegment(request.SegmentId, request.Name, request.Description, request.Slot);

            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }

            return result;
        }
    }
}

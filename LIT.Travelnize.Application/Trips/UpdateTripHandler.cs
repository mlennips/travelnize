using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Shared.Trips;

namespace LIT.Travelnize.UseCases.Trips
{
    public class UpdateTripHandler(IUnitOfWork uow) : ICommandHandler<UpdateTripCommand>
    {
        public async Task<Result> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip == null)
            {
                return TripErrors.TripNotFound;
            }
            var result = trip.Update(request.Name, request.Description,
                PlanningSlot.Create(0, request.TravelStart, request.TravelEnd));
            await uow.UpdateAsync(trip);
            return result;
        }
    }
}

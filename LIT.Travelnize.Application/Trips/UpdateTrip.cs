using LIT.Travelnize.Domain.Base;
using LIT.Travelnize.Domain.Common;
using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.UseCases.Base;

namespace LIT.Travelnize.UseCases.Trips
{
    public static class UpdateTrip
    {
        public record UpdateTripCommand(Guid TripId, string Name, string Description, DateTime TravelStart, DateTime TravelEnd) : ICommand;

        public class UpdateTripHandler(IUnitOfWork uow) : ICommandHandler<UpdateTripCommand>
        {
            public async Task<Result> Handle(UpdateTripCommand request, CancellationToken cancellationToken)
            {
                var trip = await uow.GetByIdAsync<Trip>(request.TripId);
                if (trip == null)
                {
                    return TripErrors.TripNotFound;
                }
                var result = trip.Update(request.Name, request.Description, new DateRange(request.TravelStart, request.TravelEnd));
                await uow.UpdateAsync(trip);
                return result;
            }
        }
    }
}

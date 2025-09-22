using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Trips
{
    public class RemoveParticipantHandler(IUnitOfWork uow) : ICommandHandler<RemoveParticipantCommand>
    {
        public async Task<Result> Handle(RemoveParticipantCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.RemoveParticipant(request.ParticipantId);
            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }
            return result;
        }
    }
}
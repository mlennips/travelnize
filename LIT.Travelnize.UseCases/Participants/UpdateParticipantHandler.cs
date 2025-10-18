using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Participants
{
    public class UpdateParticipantHandler(IUnitOfWork uow) : ICommandHandler<UpdateParticipantCommand>
    {
        public async Task<Result> Handle(UpdateParticipantCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.UpdateParticipant(request.ParticipantId, request.Name, request.Email);
            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }
            return result;
        }
    }
}
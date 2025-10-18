using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Participants
{
    public class ChangeParticipantPermissionHandler(IUnitOfWork uow) : ICommandHandler<ChangeParticipantPermissionCommand>
    {
        public async Task<Result> Handle(ChangeParticipantPermissionCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.ChangeParticipantPermission(request.ParticipantId, request.PermissionLevel);
            if (result.IsSuccess)
            {
                await uow.UpdateAsync(trip);
            }
            return result;
        }
    }
}
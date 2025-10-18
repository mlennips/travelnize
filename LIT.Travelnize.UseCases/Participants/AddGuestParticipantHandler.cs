using LIT.Travelnize.Domain.Trips;
using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.UseCases.Participants
{
    public class AddGuestParticipantHandler(IUnitOfWork uow) : ICommandHandler<AddGuestParticipantCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(AddGuestParticipantCommand request, CancellationToken cancellationToken)
        {
            var trip = await uow.GetByIdAsync<Trip>(request.TripId);
            if (trip is null) return TripErrors.TripNotFound;

            var result = trip.AddParticipantAsGuest(request.Name, request.Email);
            if (!result.IsSuccess) return result.Error;

            await uow.UpdateAsync(trip);
            return result.Value!.Id;
        }
    }
}
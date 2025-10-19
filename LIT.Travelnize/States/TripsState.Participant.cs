using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Result<Guid>> AddParticipantAsync(Guid tripId, AddParticipantCommand command)
            => ExecuteCreateAsync(() => _tripsApiClient.AddParticipantAsync(tripId, command), "Participant_Singular", tripId);

        public Task<Result<Guid>> AddGuestParticipantAsync(Guid tripId, AddGuestParticipantCommand command)
            => ExecuteCreateAsync(() => _tripsApiClient.AddGuestParticipantAsync(tripId, command), "Participant_Singular", tripId);

        public Task<Result<bool>> UpdateParticipantAsync(Guid tripId, Guid participantId, UpdateParticipantCommand command)
            => ExecuteMutationAsync(() => _tripsApiClient.UpdateParticipantAsync(tripId, participantId, command), "Participant_Singular", tripId);

        public Task<Result<bool>> ChangeParticipantPermissionAsync(Guid tripId, Guid participantId, ChangeParticipantPermissionCommand command)
            => ExecuteMutationAsync(() => _tripsApiClient.ChangeParticipantPermissionAsync(tripId, participantId, command), "Participant_Singular", tripId);

        public Task<Result<bool>> RemoveParticipantAsync(Guid tripId, Guid participantId)
            => ExecuteMutationAsync(() => _tripsApiClient.RemoveParticipantAsync(tripId, participantId), "Participant_Singular", tripId);
    }
}

using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Guid?> AddParticipantAsync(Guid tripId, AddParticipantCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddParticipantAsync(tripId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && _selectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            }, "Participant_Singular");

        public Task<Guid?> AddGuestParticipantAsync(Guid tripId, AddGuestParticipantCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddGuestParticipantAsync(tripId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && _selectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            }, "Participant_Singular");

        public Task<bool> UpdateParticipantAsync(Guid tripId, Guid participantId, UpdateParticipantCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.UpdateParticipantAsync(tripId, participantId, command),
                "Participant_Singular", reloadTrip: reloadTrip);

        public Task<bool> ChangeParticipantPermissionAsync(Guid tripId, Guid participantId, ChangeParticipantPermissionCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.ChangeParticipantPermissionAsync(tripId, participantId, command),
                "Participant_Singular", reloadTrip: reloadTrip);

        public Task<bool> RemoveParticipantAsync(Guid tripId, Guid participantId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.RemoveParticipantAsync(tripId, participantId),
                "Participant_Singular", reloadTrip: reloadTrip, successKey: "Success_Deleted");
    }
}

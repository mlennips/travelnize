using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Guid?> AddActivityAsync(Guid tripId, Guid destinationId, AddActivityCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddActivityAsync(tripId, destinationId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && _selectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            }, "Activity_Singular");

        public Task<bool> UpdateActivityAsync(Guid tripId, UpdateActivityCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.UpdateActivityAsync(tripId, command.DestinationId, command.ActivityId, command),
                "Activity_Singular", reloadTrip: reloadTrip);

        public Task<bool> RemoveActivityAsync(Guid tripId, Guid destinationId, Guid activityId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.RemoveActivityAsync(tripId, destinationId, activityId),
                "Activity_Singular", reloadTrip: reloadTrip, successKey: "Success_Deleted");
    }
}

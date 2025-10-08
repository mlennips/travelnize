using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Guid?> AddDestinationAsync(Guid tripId, Guid segmentId, AddDestinationCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddDestinationAsync(tripId, segmentId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && _selectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            }, "Destination_Singular");

        public Task<bool> UpdateDestinationAsync(Guid tripId, UpdateDestinationCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.UpdateDestinationAsync(tripId, command.SegmentId, command.DestinationId, command),
                "Destination_Singular", reloadTrip: reloadTrip);

        public Task<bool> RemoveDestinationAsync(Guid tripId, Guid segmentId, Guid destinationId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.RemoveDestinationAsync(tripId, segmentId, destinationId),
                "Destination_Singular", reloadTrip: reloadTrip, successKey: "Success_Deleted");

    }
}

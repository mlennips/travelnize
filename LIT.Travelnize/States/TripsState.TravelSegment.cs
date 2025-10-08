using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Guid?> AddTravelSegmentAsync(Guid tripId, AddTravelSegmentCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddTravelSegmentAsync(tripId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && _selectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            }, "TravelSegment");

        public Task<bool> UpdateTravelSegmentAsync(Guid tripId, Guid segmentId, UpdateTravelSegmentCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId, () => _tripsApiClient.UpdateTravelSegmentAsync(tripId, segmentId, command), "TravelSegment",
                reloadTrip: reloadTrip);

        public Task<bool> RemoveTravelSegmentAsync(Guid tripId, Guid segmentId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId, () => _tripsApiClient.RemoveTravelSegmentAsync(tripId, segmentId), "TravelSegment",
                reloadTrip: reloadTrip, successKey: "Success_Deleted");
    }
}

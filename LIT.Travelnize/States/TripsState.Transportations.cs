using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Guid?> AddTransportationAsync(Guid tripId, AddTransportationCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddTransportationAsync(tripId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && _selectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            }, "Transportation_Singular");

        public Task<bool> UpdateTransportationAsync(Guid tripId, Guid transportationId, UpdateTransportationCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.UpdateTransportationAsync(tripId, transportationId, command),
                "Transportation_Singular", reloadTrip: reloadTrip);

        public Task<bool> RemoveTransportationAsync(Guid tripId, Guid transportationId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.RemoveTransportationAsync(tripId, transportationId),
                "Transportation_Singular", reloadTrip: reloadTrip, successKey: "Success_Deleted");
    }
}

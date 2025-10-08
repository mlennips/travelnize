using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Guid?> CreateTripAsync(CreateTripCommand command)
            => ExecuteCreateAsync(() => _tripsApiClient.CreateTripAsync(command), "Trip", markTripsDirty: true);

        public Task<bool> UpdateTripAsync(Guid tripId, UpdateTripCommand command)
            => ExecuteMutationAsync(tripId, () => _tripsApiClient.UpdateTripAsync(tripId, command), "Trip",
                markTripsDirty: true, reloadTrip: true);

        public Task<bool> DeleteTripAsync(Guid tripId)
            => ExecuteMutationAsync(tripId, () => _tripsApiClient.DeleteTripAsync(tripId), "Trip",
                markTripsDirty: true, reloadTrip: false, successKey: "Success_Deleted");
    }
}

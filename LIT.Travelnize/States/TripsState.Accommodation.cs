using LIT.Travelnize.Domain.Trips.Commands;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Guid?> AddAccommodationAsync(Guid tripId, Guid destinationId, AddAccommodationCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddAccommodationAsync(tripId, destinationId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && _selectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            }, "Accommodation_Singular");

        public Task<bool> UpdateAccommodationAsync(Guid tripId, UpdateAccommodationCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.UpdateAccommodationAsync(tripId, command.DestinationId, command.AccommodationId, command),
                "Accommodation_Singular", reloadTrip: reloadTrip);

        public Task<bool> RemoveAccommodationAsync(Guid tripId, Guid destinationId, Guid accommodationId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.RemoveAccommodationAsync(tripId, destinationId, accommodationId),
                "Accommodation_Singular", reloadTrip: reloadTrip, successKey: "Success_Deleted");
    }
}

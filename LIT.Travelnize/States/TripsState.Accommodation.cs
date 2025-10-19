using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Result<Guid>> AddAccommodationAsync(Guid tripId, Guid destinationId, AddAccommodationCommand command)
            => ExecuteCreateAsync(() => _tripsApiClient.AddAccommodationAsync(tripId, destinationId, command), "Accommodation_Singular", tripId);

        public Task<Result<bool>> UpdateAccommodationAsync(Guid tripId, UpdateAccommodationCommand command)
            => ExecuteMutationAsync(() => _tripsApiClient.UpdateAccommodationAsync(tripId, command.DestinationId, command.AccommodationId, command), "Accommodation_Singular", tripId);

        public Task<Result<bool>> RemoveAccommodationAsync(Guid tripId, Guid destinationId, Guid accommodationId)
            => ExecuteMutationAsync(() => _tripsApiClient.RemoveAccommodationAsync(tripId, destinationId, accommodationId), "Accommodation_Singular", tripId);
    }
}

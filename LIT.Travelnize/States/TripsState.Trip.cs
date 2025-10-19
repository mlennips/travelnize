using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Result<Guid>> CreateTripAsync(CreateTripCommand command)
            => ExecuteCreateAsync(() => _tripsApiClient.CreateTripAsync(command), "Trip", null);

        public Task<Result<bool>> UpdateTripAsync(Guid tripId, UpdateTripCommand command)
            => ExecuteMutationAsync(() => _tripsApiClient.UpdateTripAsync(tripId, command), "Trip", tripId);

        public Task<Result<bool>> DeleteTripAsync(Guid tripId)
            => ExecuteMutationAsync(() => _tripsApiClient.DeleteTripAsync(tripId), "Trip", tripId);
    }
}

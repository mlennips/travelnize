using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Result<Guid>> AddTransportationAsync(Guid tripId, AddTransportationCommand command)
            => ExecuteCreateAsync(() => _tripsApiClient.AddTransportationAsync(tripId, command), "Transportation_Singular", tripId);

        public Task<Result<bool>> UpdateTransportationAsync(Guid tripId, Guid transportationId, UpdateTransportationCommand command)
            => ExecuteMutationAsync(() => _tripsApiClient.UpdateTransportationAsync(tripId, transportationId, command), "Transportation_Singular", tripId);

        public Task<Result<bool>> RemoveTransportationAsync(Guid tripId, Guid transportationId)
            => ExecuteMutationAsync(() => _tripsApiClient.RemoveTransportationAsync(tripId, transportationId), "Transportation_Singular", tripId);
    }
}

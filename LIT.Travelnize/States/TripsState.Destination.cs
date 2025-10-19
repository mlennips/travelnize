using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Result<Guid>> AddDestinationAsync(Guid tripId, Guid segmentId, AddDestinationCommand command)
            => ExecuteCreateAsync(() => _tripsApiClient.AddDestinationAsync(tripId, segmentId, command), "Destination_Singular", tripId);

        public Task<Result<bool>> UpdateDestinationAsync(Guid tripId, UpdateDestinationCommand command)
            => ExecuteMutationAsync(() => _tripsApiClient.UpdateDestinationAsync(tripId, command.SegmentId, command.DestinationId, command), "Destination_Singular", tripId);

        public Task<Result<bool>> RemoveDestinationAsync(Guid tripId, Guid segmentId, Guid destinationId)
            => ExecuteMutationAsync(() => _tripsApiClient.RemoveDestinationAsync(tripId, segmentId, destinationId), "Destination_Singular", tripId);
    }
}

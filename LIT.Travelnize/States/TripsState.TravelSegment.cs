using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Result<Guid>> AddTravelSegmentAsync(Guid tripId, AddTravelSegmentCommand command)
            => ExecuteCreateAsync(() => _tripsApiClient.AddTravelSegmentAsync(tripId, command), "TravelSegment", tripId);

        public Task<Result<bool>> UpdateTravelSegmentAsync(Guid tripId, Guid segmentId, UpdateTravelSegmentCommand command)
            => ExecuteMutationAsync(() => _tripsApiClient.UpdateTravelSegmentAsync(tripId, segmentId, command), "TravelSegment", tripId);

        public Task<Result<bool>> RemoveTravelSegmentAsync(Guid tripId, Guid segmentId)
            => ExecuteMutationAsync(() => _tripsApiClient.RemoveTravelSegmentAsync(tripId, segmentId), "TravelSegment", tripId);
    }
}

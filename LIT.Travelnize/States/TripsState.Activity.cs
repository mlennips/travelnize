using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.States
{
    public partial class TripsState
    {
        public Task<Result<Guid>> AddActivityAsync(Guid tripId, Guid destinationId, AddActivityCommand command)
            => ExecuteCreateAsync(() => _tripsApiClient.AddActivityAsync(tripId, destinationId, command), "Activity_Singular", tripId);

        public Task<Result<bool>> UpdateActivityAsync(Guid tripId, UpdateActivityCommand command)
            => ExecuteMutationAsync(() => _tripsApiClient.UpdateActivityAsync(tripId, command.DestinationId, command.ActivityId, command), "Activity_Singular", tripId);

        public Task<Result<bool>> RemoveActivityAsync(Guid tripId, Guid destinationId, Guid activityId)
            => ExecuteMutationAsync(() => _tripsApiClient.RemoveActivityAsync(tripId, destinationId, activityId), "Activity_Singular", tripId);
    }
}

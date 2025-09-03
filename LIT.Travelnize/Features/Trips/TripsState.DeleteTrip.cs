using LIT.Travelnize.Services.Api;
using TimeWarp.State;

namespace LIT.Travelnize.Features.Trips
{
    partial class TripsState
    {
        public record DeleteTripAction(Guid TripId) : IAction;
        public class DeleteTripHandler(IStore store, TripsApiClient apiClient) : ActionHandler<DeleteTripAction>(store)
        {
            public override async Task Handle(DeleteTripAction action, CancellationToken cancellationToken)
            {
                var state = Store.GetState<TripsState>();
                state.IsLoading = true;
                Store.SetState(state);

                await apiClient.DeleteTripAsync(action.TripId);
                state.IsLoading = false;
                Store.SetState(state);
            }
        }
    }
}

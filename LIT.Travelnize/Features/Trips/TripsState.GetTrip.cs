using LIT.Travelnize.Services.Api;
using TimeWarp.State;

namespace LIT.Travelnize.Features.Trips
{
    partial class TripsState
    {
        public record GetTripAction(Guid TripId) : IAction;
        public class GetTripHandler(IStore store, TripsApiClient apiClient) : ActionHandler<GetTripAction>(store)
        {
            public override async Task Handle(GetTripAction action, CancellationToken cancellationToken)
            {
                var state = Store.GetState<TripsState>();
                state.IsLoading = true;
                state.SelectedTrip = null;
                Store.SetState(state);

                var trip = await apiClient.GetTripAsync(action.TripId);
                state.IsLoading = false;
                state.SelectedTrip = trip;
                Store.SetState(state);
            }
        }
    }
}

using LIT.Travelnize.Services.Api;
using LIT.Travelnize.Shared.Trips;
using TimeWarp.State;

namespace LIT.Travelnize.Features.Trips
{
    partial class TripsState
    {
        public record UpdateTripAction(Guid TripId, UpdateTripCommand Command) : IAction;
        public class UpdateTripHandler(IStore store, TripsApiClient apiClient) : ActionHandler<UpdateTripAction>(store)
        {
            public override async Task Handle(UpdateTripAction action, CancellationToken cancellationToken)
            {
                var state = Store.GetState<TripsState>();
                state.IsLoading = true;
                Store.SetState(state);

                await apiClient.UpdateTripAsync(action.TripId, action.Command);
                state.IsLoading = false;
                Store.SetState(state);
            }
        }
    }
}

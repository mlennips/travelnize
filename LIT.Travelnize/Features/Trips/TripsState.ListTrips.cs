namespace LIT.Travelnize.Features.Trips
{
    partial class TripState
    {
		public record ListTripsAction(Guid UserId) : IAction;
		public class ListTripsHandler(IStore store, TripsApiClient apiClient) : ActionHandler<ListTripsAction>(store)
		{
			public override async Task Handle(ListTripsAction action, CancellationToken cancellationToken)
			{
				var state = Store.GetState<TripsState>();
				state.IsLoading = true;
				state.Trips = null;
				state.SelectedTrip = null;
				Store.SetState(state);

				var trips = await apiClient.GetTripsAsync(action.UserId) ?? [];
				state.IsLoading = false;
				state.Trips = trips;
				Store.SetState(state);
			}
		}
	}
}

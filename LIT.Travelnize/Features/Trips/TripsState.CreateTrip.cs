namespace LIT.Travelnize.Features.Trips
{
    partial class TripsState
    {
		public record CreateTripAction(CreateTripCommand Command) : IAction;
		public class CreateTripHandler(IStore store, TripsApiClient apiClient) : ActionHandler<CreateTripAction>(store)
		{
			public override async Task Handle(CreateTripAction action, CancellationToken cancellationToken)
			{
				var state = Store.GetState<TripsState>();
				state.IsLoading = true;
				Store.SetState(state);

				await apiClient.CreateTripAsync(action.Command);
				state.IsLoading = false;
				Store.SetState(state);
			}
		}
	}
}

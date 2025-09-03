using Fluxor;
using LIT.Travelnize.Services.Api;

namespace LIT.Travelnize.Store.Trips
{
    public class TripEffects(TripsApiClient api)
    {
        [EffectMethod]
        public async Task HandleListTrips(ListTripsAction action, IDispatcher dispatcher)
        {
            try
            {
                var trips = await api.GetTripsAsync(action.Query.UserId);
                dispatcher.Dispatch(new ListTripsResultAction(trips ?? []));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new TripErrorAction(ex.Message));
            }
        }

        [EffectMethod]
        public async Task HandleGetTrip(GetTripAction action, IDispatcher dispatcher)
        {
            try
            {
                var trip = await api.GetTripAsync(action.Query.TripId);
                dispatcher.Dispatch(new GetTripResultAction(trip!));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new TripErrorAction(ex.Message));
            }
        }

        [EffectMethod]
        public async Task HandleCreateTrip(CreateTripAction action, IDispatcher dispatcher)
        {
            try
            {
                var trip = await api.CreateTripAsync(action.Command);
                //dispatcher.Dispatch(new CreateTripResultAction(trip));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new TripErrorAction(ex.Message));
            }
        }

        [EffectMethod]
        public Task HandleUpdateTrip(UpdateTripAction action, IDispatcher dispatcher)
        {
            try
            {
                //var trip = await _api.UpdateTripAsync(action.Command);
                //dispatcher.Dispatch(new UpdateTripResultAction(trip));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new TripErrorAction(ex.Message));
            }
            return Task.CompletedTask;
        }

        [EffectMethod]
        public Task HandleDeleteTrip(DeleteTripAction action, IDispatcher dispatcher)
        {
            try
            {
                //await _api.DeleteTripAsync(action.Command);
                //dispatcher.Dispatch(new DeleteTripResultAction(action.Command.TripId));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new TripErrorAction(ex.Message));
            }
            return Task.CompletedTask;
        }
    }
}
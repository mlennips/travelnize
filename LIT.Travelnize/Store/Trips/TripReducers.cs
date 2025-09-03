using Fluxor;

namespace LIT.Travelnize.Store.Trips
{
    public static class TripReducers
    {
        [ReducerMethod]
        public static TripState OnListTrips(TripState state, ListTripsAction action) =>
            state with { IsLoading = true, ErrorMessage = null };

        [ReducerMethod]
        public static TripState OnListTripsResult(TripState state, ListTripsResultAction action) =>
            state with { IsLoading = false, Trips = action.Trips, ErrorMessage = null };

        [ReducerMethod]
        public static TripState OnGetTripResult(TripState state, GetTripResultAction action) =>
            state with { IsLoading = false, CurrentTrip = action.Trip, ErrorMessage = null };

        [ReducerMethod]
        public static TripState OnCreateTripResult(TripState state, CreateTripResultAction action) =>
            state with { 
                IsLoading = false, 
                //Trips = state.Trips.Append(action.Trip).ToArray(), 
                ErrorMessage = null 
            };

        [ReducerMethod]
        public static TripState OnUpdateTripResult(TripState state, UpdateTripResultAction action) =>
            state with
            {
                IsLoading = false,
                //Trips = state.Trips.Select(t => t.Id == action. ? action.Trip : t).ToArray(),
                ErrorMessage = null
            };

        [ReducerMethod]
        public static TripState OnDeleteTripResult(TripState state, DeleteTripResultAction action) =>
            state with
            {
                IsLoading = false,
                //Trips = state.Trips.Where(t => t.Id != action.TripId).ToArray(),
                ErrorMessage = null
            };

        [ReducerMethod]
        public static TripState OnError(TripState state, TripErrorAction action) =>
            state with { IsLoading = false, ErrorMessage = action.ErrorMessage };
    }
}
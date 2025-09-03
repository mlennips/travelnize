using Fluxor;
using LIT.Travelnize.Domain.Trips.Queries;

namespace LIT.Travelnize.Store.Trips
{
    [FeatureState]
    public record TripState(
        bool IsLoading,
        ListTripsDto[] Trips,
        GetTripDto? CurrentTrip,
        string? ErrorMessage
    )
    {
        public TripState() : this(true, [], null, null)
        {

        }
    }
}
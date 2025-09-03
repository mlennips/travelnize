using LIT.Travelnize.Services.Api;
using LIT.Travelnize.Shared.Trips;
using TimeWarp.State;

namespace LIT.Travelnize.Features.Trips
{
    public sealed partial class TripsState : State<TripsState>
    {
        public bool IsLoading { get; private set; } = false;
        public ListTripsDto[]? Trips { get; private set; }
        public GetTripDto? SelectedTrip { get; private set; }

        public override void Initialize()
        {
            IsLoading = false;
            Trips = [];
            SelectedTrip = null;
        }
    }
}
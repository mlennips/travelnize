using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.Queries;
using LIT.Travelnize.Domain.Trips.ValueObjects;
using LIT.Travelnize.Services.Api;

namespace LIT.Travelnize.States
{
    public class TripsState(TripsApiClient _tripsApiClient)
    {
        private bool _needsRefresh = true;

        public event Action? OnChange;

        public bool IsLoading { get; set; }

        private GetTripDto? _selectedTrip;
        public GetTripDto? SelectedTrip
        {
            get => _selectedTrip;
            set
            {
                if (_selectedTrip != value)
                {
                    _selectedTrip = value;
                }
            }
        }

        private ListTripsDto[]? _trips;
        public ListTripsDto[]? Trips
        {
            get => _trips;
            private set
            {
                if (_trips != value)
                {
                    _trips = value;
                }
            }
        }

        public IEnumerable<ListTripsDto> OngoingTrips =>
            Trips?.Where(t => t.Status == TripStatus.Ongoing).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsDto>();

        public IEnumerable<ListTripsDto> UpcomingTrips =>
            Trips?.Where(t => t.Status == TripStatus.Upcoming).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsDto>();

        public async Task LoadTripsAsync(Guid userId)
        {
            if (!_needsRefresh && Trips != null)
            {
                return;
            }
            IsLoading = true;
            NotifyStateChanged();
            Trips = await _tripsApiClient.GetTripsAsync(userId);
            IsLoading = false;
            NotifyStateChanged();
        }

        public async Task LoadTripAsync(Guid tripId)
        {
            IsLoading = true;
            NotifyStateChanged();
            SelectedTrip = await _tripsApiClient.GetTripAsync(tripId);
            IsLoading = false;
            NotifyStateChanged();
        }

        public async Task<Guid?> CreateTripAsync(CreateTripCommand command)
        {
            var tripId = await _tripsApiClient.CreateTripAsync(command);
            _needsRefresh = true;
            // Optional: Nach dem Erstellen die Liste neu laden
            // await LoadTripsAsync(command.UserId);
            return tripId;
        }

        public async Task<bool> UpdateTripAsync(Guid tripId, UpdateTripCommand command)
        {
            var result = await _tripsApiClient.UpdateTripAsync(tripId, command);
            if (result)
            {
                _needsRefresh = true;
                // Optional: Nach dem Update den Trip neu laden
                await LoadTripAsync(tripId);
            }
            return result;
        }

        public async Task<bool> DeleteTripAsync(Guid tripId)
        {
            var result = await _tripsApiClient.DeleteTripAsync(tripId);
            if (result)
            {
                _needsRefresh = true;
                // Optional: Nach dem Löschen die Liste neu laden
                // Trips = Trips?.Where(t => t.Id != tripId).ToArray();
            }
            return result;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}

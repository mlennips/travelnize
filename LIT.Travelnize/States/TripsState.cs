using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.Queries;
using LIT.Travelnize.Domain.Trips.ValueObjects;
using LIT.Travelnize.Services.Api;

namespace LIT.Travelnize.States
{
    public class TripsState(TripsApiClient _tripsApiClient)
    {
        #region State Fields & Events
        private bool _needsRefresh = true;

        public event Action? OnChange;

        public bool IsLoading { get; set; }
        #endregion

        #region Properties
        private GetTripResponse? _selectedTrip;
        public GetTripResponse? SelectedTrip
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

        private ListTripsResponse[]? _trips;
        public ListTripsResponse[]? Trips
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

        public IEnumerable<ListTripsResponse> OngoingTrips =>
            Trips?.Where(t => t.Status == TripStatus.Ongoing).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsResponse>();

        public IEnumerable<ListTripsResponse> UpcomingTrips =>
            Trips?.Where(t => t.Status == TripStatus.Upcoming).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsResponse>();
        #endregion

        #region Loading
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
            _needsRefresh = false;
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
        #endregion

        #region Trips
        public async Task<Guid?> CreateTripAsync(CreateTripCommand command)
        {
            var tripId = await _tripsApiClient.CreateTripAsync(command);
            if (tripId.HasValue)
            {
                _needsRefresh = true;
            }
            return tripId;
        }

        public async Task<bool> UpdateTripAsync(Guid tripId, UpdateTripCommand command)
        {
            var result = await _tripsApiClient.UpdateTripAsync(tripId, command);
            if (result)
            {
                _needsRefresh = true;
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
                if (SelectedTrip?.Id == tripId)
                {
                    SelectedTrip = null;
                    NotifyStateChanged();
                }
            }
            return result;
        }
        #endregion

        #region Travel Segments
        public async Task<Guid?> AddTravelSegmentAsync(Guid tripId, AddTravelSegmentCommand command, bool reloadTrip = true)
        {
            var id = await _tripsApiClient.AddTravelSegmentAsync(tripId, command);
            if (id.HasValue)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return id;
        }

        public async Task<bool> UpdateTravelSegmentAsync(Guid tripId, Guid segmentId, UpdateTravelSegmentCommand command, bool reloadTrip = true)
        {
            var result = await _tripsApiClient.UpdateTravelSegmentAsync(tripId, segmentId, command);
            if (result)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return result;
        }

        public async Task<bool> RemoveTravelSegmentAsync(Guid tripId, Guid segmentId, bool reloadTrip = true)
        {
            var result = await _tripsApiClient.RemoveTravelSegmentAsync(tripId, segmentId);
            if (result)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return result;
        }
        #endregion

        #region Destinations
        public async Task<Guid?> AddDestinationAsync(Guid tripId, Guid segmentId, AddDestinationCommand command, bool reloadTrip = true)
        {
            var id = await _tripsApiClient.AddDestinationAsync(tripId, segmentId, command);
            if (id.HasValue)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return id;
        }

        public async Task<bool> UpdateDestinationAsync(Guid tripId, UpdateDestinationCommand command, bool reloadTrip = true)
        {
            var result = await _tripsApiClient.UpdateDestinationAsync(tripId, command.SegmentId, command.DestinationId, command);
            if (result)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return result;
        }

        public async Task<bool> RemoveDestinationAsync(Guid tripId, Guid segmentId, Guid destinationId, bool reloadTrip = true)
        {
            var result = await _tripsApiClient.RemoveDestinationAsync(tripId, segmentId, destinationId);
            if (result)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return result;
        }
        #endregion

        #region Participants
        public async Task<Guid?> AddParticipantAsync(Guid tripId, AddParticipantCommand command, bool reloadTrip = true)
        {
            var id = await _tripsApiClient.AddParticipantAsync(tripId, command);
            if (id.HasValue)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return id;
        }

        public async Task<Guid?> AddGuestParticipantAsync(Guid tripId, AddGuestParticipantCommand command, bool reloadTrip = true)
        {
            var id = await _tripsApiClient.AddGuestParticipantAsync(tripId, command);
            if (id.HasValue)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return id;
        }

        public async Task<bool> UpdateParticipantAsync(Guid tripId, Guid participantId, UpdateParticipantCommand command, bool reloadTrip = true)
        {
            var result = await _tripsApiClient.UpdateParticipantAsync(tripId, participantId, command);
            if (result)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return result;
        }

        public async Task<bool> ChangeParticipantPermissionAsync(Guid tripId, Guid participantId, ChangeParticipantPermissionCommand command, bool reloadTrip = true)
        {
            var result = await _tripsApiClient.ChangeParticipantPermissionAsync(tripId, participantId, command);
            if (result)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return result;
        }

        public async Task<bool> RemoveParticipantAsync(Guid tripId, Guid participantId, bool reloadTrip = true)
        {
            var result = await _tripsApiClient.RemoveParticipantAsync(tripId, participantId);
            if (result)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return result;
        }
        #endregion

        #region Transportation
        public async Task<Guid?> AddTransportationAsync(Guid tripId, AddTransportationCommand command, bool reloadTrip = true)
        {
            var id = await _tripsApiClient.AddTransportationAsync(tripId, command);
            if (id.HasValue)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return id;
        }
        #endregion

        #region Accommodation
        public async Task<Guid?> AddAccommodationAsync(Guid tripId, Guid destinationId, AddAccommodationCommand command, bool reloadTrip = true)
        {
            var id = await _tripsApiClient.AddAccommodationAsync(tripId, destinationId, command);
            if (id.HasValue)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return id;
        }
        #endregion

        #region Activities
        public async Task<Guid?> AddActivityAsync(Guid tripId, Guid destinationId, AddActivityCommand command, bool reloadTrip = true)
        {
            var id = await _tripsApiClient.AddActivityAsync(tripId, destinationId, command);
            if (id.HasValue)
            {
                _needsRefresh = true;
                if (reloadTrip) await LoadTripIfCurrentlySelected(tripId);
            }
            return id;
        }
        #endregion

        #region Helpers
        private async Task LoadTripIfCurrentlySelected(Guid tripId)
        {
            if (SelectedTrip?.Id == tripId)
            {
                await LoadTripAsync(tripId);
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
        #endregion
    }
}

using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.Queries;
using LIT.Travelnize.Domain.Trips.ValueObjects;
using LIT.Travelnize.Services.Api;
using System.Diagnostics;

namespace LIT.Travelnize.States
{
    public class TripsState(TripsApiClient _tripsApiClient, ILogger<TripsState> _logger)
    {
        #region State Fields & Events
        private bool _tripsDirty = true;
        private readonly HashSet<Guid> _dirtyTripIds = [];
        private bool _listLoading;
        private bool _tripLoading;

        public event Action? OnChange;
        #endregion

        #region Public State
        public bool IsListLoading
        {
            get => _listLoading;
            private set
            {
                if (_listLoading == value) return;
                _listLoading = value;
                NotifyStateChanged();
            }
        }

        public bool IsTripLoading
        {
            get => _tripLoading;
            private set
            {
                if (_tripLoading == value) return;
                _tripLoading = value;
                NotifyStateChanged();
            }
        }

        public bool IsLoading => IsListLoading || IsTripLoading;

        public string? LastError { get; private set; }
        #endregion

        #region Properties
        private GetTripResponse? _selectedTrip;
        public GetTripResponse? SelectedTrip
        {
            get => _selectedTrip;
            private set
            {
                if (_selectedTrip == value) return;
                _selectedTrip = value;
                NotifyStateChanged();
            }
        }

        private ListTripsResponse[]? _trips;
        public ListTripsResponse[]? Trips
        {
            get => _trips;
            private set
            {
                if (_trips == value) return;
                _trips = value;
                NotifyStateChanged();
            }
        }

        public IEnumerable<ListTripsResponse> OngoingTrips =>
            Trips?.Where(t => t.Status == TripStatus.Ongoing).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsResponse>();
        public IEnumerable<ListTripsResponse> UpcomingTrips =>
            Trips?.Where(t => t.Status == TripStatus.Upcoming).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsResponse>();
        public IEnumerable<ListTripsResponse> PastTrips =>
            Trips?.Where(t => t.Status == TripStatus.Past).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsResponse>();
        public IEnumerable<ListTripsResponse> PendingTrips =>
            Trips?.Where(t => t.Status == TripStatus.Pending).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsResponse>();
        #endregion

        #region Loading
        public async Task LoadTripsAsync(Guid userId, bool force = false)
        {
            if (!force && !_tripsDirty && Trips is not null)
            {
                _logger.LogDebug("Trips nicht geladen (Cache gültig) für User {UserId}", userId);
                return;
            }

            LastError = null;
            IsListLoading = true;
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("Lade Trip-Liste für User {UserId} (force={Force})...", userId, force);
            try
            {
                Trips = await _tripsApiClient.GetTripsAsync(userId);
                _tripsDirty = false;
                sw.Stop();
                _logger.LogInformation("Trip-Liste geladen: {Count} Einträge für User {UserId} (in {Elapsed} ms)",
                    Trips?.Length ?? 0, userId, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                LastError = ex.Message;
                _logger.LogError(ex, "Fehler beim Laden der Trip-Liste für User {UserId} nach {Elapsed} ms", userId, sw.ElapsedMilliseconds);
            }
            finally
            {
                IsListLoading = false;
            }
        }

        public async Task LoadTripAsync(Guid tripId, bool force = false)
        {
            if (!force && !_dirtyTripIds.Contains(tripId) && SelectedTrip?.Id == tripId)
            {
                _logger.LogDebug("Trip {TripId} nicht neu geladen (Detailcache gültig)", tripId);
                return;
            }

            LastError = null;
            IsTripLoading = true;
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("Lade Trip {TripId} (force={Force})...", tripId, force);
            try
            {
                SelectedTrip = await _tripsApiClient.GetTripAsync(tripId);
                _dirtyTripIds.Remove(tripId);
                sw.Stop();
                if (SelectedTrip is null)
                {
                    _logger.LogWarning("Trip {TripId} nicht gefunden (Ladezeit {Elapsed} ms)", tripId, sw.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogInformation("Trip {TripId} geladen (Ladezeit {Elapsed} ms, Segmente={Segments}, Participants={Participants})",
                        tripId, sw.ElapsedMilliseconds, SelectedTrip.TravelSegments.Count, SelectedTrip.Participants.Count);
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                LastError = ex.Message;
                _logger.LogError(ex, "Fehler beim Laden von Trip {TripId} nach {Elapsed} ms", tripId, sw.ElapsedMilliseconds);
            }
            finally
            {
                IsTripLoading = false;
            }
        }
        #endregion

        #region Helper Mutations
        private async Task<TResult?> ExecuteCreateAsync<TResult>(Func<Task<TResult?>> action, bool markTripsDirty = false)
        {
            try
            {
                var id = await action();
                if (id is not null && markTripsDirty)
                {
                    _tripsDirty = true;
                    _logger.LogDebug("Trip-Liste als dirty markiert (Create)");
                }
                return id;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                _logger.LogError(ex, "Fehler bei Create-Operation");
                NotifyStateChanged();
                return default;
            }
        }

        private async Task<bool> ExecuteMutationAsync(Guid tripId, Func<Task<bool>> action,
            bool markTripsDirty = false, bool reloadTrip = true)
        {
            try
            {
                var ok = await action();
                if (ok)
                {
                    if (markTripsDirty)
                    {
                        _tripsDirty = true;
                        _logger.LogDebug("Trip-Liste als dirty markiert (Mutation)");
                    }
                    _dirtyTripIds.Add(tripId);
                    _logger.LogDebug("Trip {TripId} als dirty markiert", tripId);

                    if (reloadTrip && SelectedTrip?.Id == tripId)
                    {
                        _logger.LogDebug("Trip {TripId} nach Mutation neu laden...", tripId);
                        await LoadTripAsync(tripId, force: true);
                    }
                }
                return ok;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                _logger.LogError(ex, "Fehler bei Mutation für Trip {TripId}", tripId);
                NotifyStateChanged();
                return false;
            }
        }
        #endregion

        #region Trips
        public Task<Guid?> CreateTripAsync(CreateTripCommand command)
            => ExecuteCreateAsync(() => _tripsApiClient.CreateTripAsync(command), markTripsDirty: true);

        public Task<bool> UpdateTripAsync(Guid tripId, UpdateTripCommand command)
            => ExecuteMutationAsync(tripId, () => _tripsApiClient.UpdateTripAsync(tripId, command),
                markTripsDirty: true, reloadTrip: true);

        public Task<bool> DeleteTripAsync(Guid tripId)
            => ExecuteMutationAsync(tripId, () => _tripsApiClient.DeleteTripAsync(tripId),
                markTripsDirty: true, reloadTrip: false);
        #endregion

        #region Travel Segments
        public Task<Guid?> AddTravelSegmentAsync(Guid tripId, AddTravelSegmentCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddTravelSegmentAsync(tripId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && SelectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            });

        public Task<bool> UpdateTravelSegmentAsync(Guid tripId, Guid segmentId, UpdateTravelSegmentCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId, () => _tripsApiClient.UpdateTravelSegmentAsync(tripId, segmentId, command),
                reloadTrip: reloadTrip);

        public Task<bool> RemoveTravelSegmentAsync(Guid tripId, Guid segmentId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId, () => _tripsApiClient.RemoveTravelSegmentAsync(tripId, segmentId),
                reloadTrip: reloadTrip);
        #endregion

        #region Destinations
        public Task<Guid?> AddDestinationAsync(Guid tripId, Guid segmentId, AddDestinationCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddDestinationAsync(tripId, segmentId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && SelectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            });

        public Task<bool> UpdateDestinationAsync(Guid tripId, UpdateDestinationCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.UpdateDestinationAsync(tripId, command.SegmentId, command.DestinationId, command),
                reloadTrip: reloadTrip);

        public Task<bool> RemoveDestinationAsync(Guid tripId, Guid segmentId, Guid destinationId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.RemoveDestinationAsync(tripId, segmentId, destinationId),
                reloadTrip: reloadTrip);
        #endregion

        #region Participants
        public Task<Guid?> AddParticipantAsync(Guid tripId, AddParticipantCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddParticipantAsync(tripId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && SelectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            });

        public Task<Guid?> AddGuestParticipantAsync(Guid tripId, AddGuestParticipantCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddGuestParticipantAsync(tripId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && SelectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            });

        public Task<bool> UpdateParticipantAsync(Guid tripId, Guid participantId, UpdateParticipantCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.UpdateParticipantAsync(tripId, participantId, command),
                reloadTrip: reloadTrip);

        public Task<bool> ChangeParticipantPermissionAsync(Guid tripId, Guid participantId, ChangeParticipantPermissionCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.ChangeParticipantPermissionAsync(tripId, participantId, command),
                reloadTrip: reloadTrip);

        public Task<bool> RemoveParticipantAsync(Guid tripId, Guid participantId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.RemoveParticipantAsync(tripId, participantId),
                reloadTrip: reloadTrip);
        #endregion

        #region Transportation
        public Task<Guid?> AddTransportationAsync(Guid tripId, AddTransportationCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddTransportationAsync(tripId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && SelectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            });

        public Task<bool> UpdateTransportationAsync(Guid tripId, Guid transportationId, UpdateTransportationCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.UpdateTransportationAsync(tripId, transportationId, command),
                reloadTrip: reloadTrip);

        public Task<bool> RemoveTransportationAsync(Guid tripId, Guid transportationId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.RemoveTransportationAsync(tripId, transportationId),
                reloadTrip: reloadTrip);
        #endregion

        #region Accommodation
        public Task<Guid?> AddAccommodationAsync(Guid tripId, Guid destinationId, AddAccommodationCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddAccommodationAsync(tripId, destinationId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && SelectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            });

        public Task<bool> UpdateAccommodationAsync(Guid tripId, UpdateAccommodationCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.UpdateAccommodationAsync(tripId, command.DestinationId, command.AccommodationId, command),
                reloadTrip: reloadTrip);

        public Task<bool> RemoveAccommodationAsync(Guid tripId, Guid destinationId, Guid accommodationId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.RemoveAccommodationAsync(tripId, destinationId, accommodationId),
                reloadTrip: reloadTrip);
        #endregion

        #region Activities
        public Task<Guid?> AddActivityAsync(Guid tripId, Guid destinationId, AddActivityCommand command, bool reloadTrip = true)
            => ExecuteCreateAsync(async () =>
            {
                var id = await _tripsApiClient.AddActivityAsync(tripId, destinationId, command);
                if (id.HasValue)
                {
                    _dirtyTripIds.Add(tripId);
                    if (reloadTrip && SelectedTrip?.Id == tripId)
                        await LoadTripAsync(tripId, force: true);
                }
                return id;
            });

        public Task<bool> UpdateActivityAsync(Guid tripId, UpdateActivityCommand command, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.UpdateActivityAsync(tripId, command.DestinationId, command.ActivityId, command),
                reloadTrip: reloadTrip);

        public Task<bool> RemoveActivityAsync(Guid tripId, Guid destinationId, Guid activityId, bool reloadTrip = true)
            => ExecuteMutationAsync(tripId,
                () => _tripsApiClient.RemoveActivityAsync(tripId, destinationId, activityId),
                reloadTrip: reloadTrip);
        #endregion

        #region Helpers
        public void MarkTripsDirty() => _tripsDirty = true;
        public void MarkTripDirty(Guid tripId) => _dirtyTripIds.Add(tripId);

        private void NotifyStateChanged() => OnChange?.Invoke();
        #endregion
    }
}

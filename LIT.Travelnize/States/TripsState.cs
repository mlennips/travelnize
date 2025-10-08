using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.Queries;
using LIT.Travelnize.Domain.Trips.ValueObjects;
using LIT.Travelnize.Interfaces;
using LIT.Travelnize.Services.Api;
using System.Diagnostics;

namespace LIT.Travelnize.States
{
    public partial class TripsState(TripsApiClient _tripsApiClient, ILogger<TripsState> _logger, INotificationService _notificationService)
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

        public bool IsLoading => _listLoading || _tripLoading;

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
            _trips?.Where(t => t.Status == TripStatus.Ongoing).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsResponse>();
        public IEnumerable<ListTripsResponse> UpcomingTrips =>
            _trips?.Where(t => t.Status == TripStatus.Upcoming).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsResponse>();
        public IEnumerable<ListTripsResponse> PastTrips =>
            _trips?.Where(t => t.Status == TripStatus.Past).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsResponse>();
        public IEnumerable<ListTripsResponse> PendingTrips =>
            _trips?.Where(t => t.Status == TripStatus.Pending).OrderBy(t => t.Slot.Start) ?? Enumerable.Empty<ListTripsResponse>();
        #endregion

        #region Loading
        public async Task LoadTripsAsync(Guid userId, bool force = false)
        {
            if (!force && !_tripsDirty && _trips is not null)
            {
                _logger.LogDebug("Trips nicht geladen (Cache gültig) für User {UserId}", userId);
                return;
            }

            LastError = null;
            IsListLoading = true;
            var sw = Stopwatch.StartNew();
            _logger.LogDebug("Lade Trip-Liste für User {UserId} (force={Force})...", userId, force);
            try
            {
                _trips = await _tripsApiClient.GetTripsAsync(userId);
                _tripsDirty = false;
                sw.Stop();
                _logger.LogInformation("Trip-Liste geladen: {Count} Einträge für User {UserId} (in {Elapsed} ms)",
                    _trips?.Length ?? 0, userId, sw.ElapsedMilliseconds);
                _notificationService.Info("Success_TripsStateRefreshed");
            }
            catch (Exception ex)
            {
                sw.Stop();
                LastError = ex.Message;
                _logger.LogError(ex, "Fehler beim Laden der Trip-Liste für User {UserId} nach {Elapsed} ms", userId, sw.ElapsedMilliseconds);
                _notificationService.Error("Errors_Load");
            }
            finally
            {
                IsListLoading = false;
                NotifyStateChanged();
            }
        }

        public async Task LoadTripAsync(Guid tripId, bool force = false)
        {
            if (!force && !_dirtyTripIds.Contains(tripId) && _selectedTrip?.Id == tripId)
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
                _selectedTrip = await _tripsApiClient.GetTripAsync(tripId);
                _dirtyTripIds.Remove(tripId);
                sw.Stop();
                if (_selectedTrip is null)
                {
                    _logger.LogWarning("Trip {TripId} nicht gefunden (Ladezeit {Elapsed} ms)", tripId, sw.ElapsedMilliseconds);
                    _notificationService.Error("Errors_TripNotFound");
                }
                else
                {
                    _logger.LogInformation("Trip {TripId} geladen (Ladezeit {Elapsed} ms, Segmente={Segments}, Participants={Participants})",
                        tripId, sw.ElapsedMilliseconds, _selectedTrip.TravelSegments.Count, _selectedTrip.Participants.Count);
                    _notificationService.Info("Success_TripStateRefreshed");
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                LastError = ex.Message;
                _logger.LogError(ex, "Fehler beim Laden von Trip {TripId} nach {Elapsed} ms", tripId, sw.ElapsedMilliseconds);
                _notificationService.Error("Errors_Load");
            }
            finally
            {
                IsTripLoading = false;
                NotifyStateChanged();
            }
        }
        #endregion

        #region Helper Mutations
        private async Task<TResult?> ExecuteCreateAsync<TResult>(Func<Task<TResult?>> action, string entitySingular, bool markTripsDirty = false)
        {
            try
            {
                var id = await action();
                if (id is not null && markTripsDirty)
                {
                    _tripsDirty = true;
                    _logger.LogDebug("{entitySingular} erstellt und Trip-Liste als dirty markiert (Create)", entitySingular);
                    _notificationService.Success("Success_Created", entitySingular);
                }
                return id;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                _logger.LogError(ex, "Fehler bei Create-Operation für {entitySingular}", entitySingular);
                _notificationService.Error("Errors_Generic", entitySingular);
                NotifyStateChanged();
                return default;
            }
        }

        private async Task<bool> ExecuteMutationAsync(Guid tripId, Func<Task<bool>> action, string entitySingular,
            bool markTripsDirty = false, bool reloadTrip = true, string? successKey = null, string? errorKey = null)
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

                    if (reloadTrip && _selectedTrip?.Id == tripId)
                    {
                        _logger.LogDebug("Trip {TripId} nach Mutation neu laden...", tripId);
                        await LoadTripAsync(tripId, force: true);
                    }
                    _notificationService.Success(successKey ?? "Success_Updated", entitySingular);
                }
                else
                {
                    _notificationService.Error(errorKey ?? "Errors_UpdateFailed", entitySingular);
                }
                return ok;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                _logger.LogError(ex, "Fehler bei Mutation für Trip {tripId} ({entitySingular})", tripId, entitySingular);
                _notificationService.Error("Errors_Generic", entitySingular);
                NotifyStateChanged();
                return false;
            }
        }
        #endregion

        #region Helpers
        public void MarkTripsDirty() => _tripsDirty = true;
        public void MarkTripDirty(Guid tripId) => _dirtyTripIds.Add(tripId);

        private void NotifyStateChanged() => OnChange?.Invoke();
        #endregion
    }
}

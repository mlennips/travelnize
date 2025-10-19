using System.Net.Http.Json;
using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.Queries;
using LIT.Travelnize.Domain.Common;

namespace LIT.Travelnize.Services.Api
{
    public class TripsApiClient(HttpClient httpClient)
    {
        public async Task<Result<Guid>> CreateTripAsync(CreateTripCommand command)
            => await HandleApiResponseAsync<Guid>(await httpClient.PostAsJsonAsync("trips", command));

        public async Task<Result<ListTripsResponse[]>> GetTripsAsync(Guid userId)
            => await HandleApiResponseAsync<ListTripsResponse[]>(await httpClient.GetAsync($"trips?userId={userId}"));

        public async Task<Result<GetTripResponse>> GetTripAsync(Guid tripId)
            => await HandleApiResponseAsync<GetTripResponse>(await httpClient.GetAsync($"trips/{tripId}"));

        public async Task<Result<bool>> UpdateTripAsync(Guid tripId, UpdateTripCommand command)
            => await HandleApiResponseAsync<bool>(await httpClient.PutAsJsonAsync($"trips/{tripId}", command));

        public async Task<Result<bool>> DeleteTripAsync(Guid tripId)
            => await HandleApiResponseAsync<bool>(await httpClient.DeleteAsync($"trips/{tripId}"));

        #region Travel Segments
        public async Task<Result<Guid>> AddTravelSegmentAsync(Guid tripId, AddTravelSegmentCommand command)
            => await HandleApiResponseAsync<Guid>(await httpClient.PostAsJsonAsync($"trips/{tripId}/segments", command));

        public async Task<Result<bool>> UpdateTravelSegmentAsync(Guid tripId, Guid segmentId, UpdateTravelSegmentCommand command)
            => await HandleApiResponseAsync<bool>(await httpClient.PutAsJsonAsync($"trips/{tripId}/segments/{segmentId}", command));

        public async Task<Result<bool>> RemoveTravelSegmentAsync(Guid tripId, Guid segmentId)
            => await HandleApiResponseAsync<bool>(await httpClient.DeleteAsync($"trips/{tripId}/segments/{segmentId}"));
        #endregion

        #region Destinations
        public async Task<Result<Guid>> AddDestinationAsync(Guid tripId, Guid segmentId, AddDestinationCommand command)
            => await HandleApiResponseAsync<Guid>(await httpClient.PostAsJsonAsync($"trips/{tripId}/segments/{segmentId}/destinations", command));

        public async Task<Result<bool>> UpdateDestinationAsync(Guid tripId, Guid segmentId, Guid destinationId, UpdateDestinationCommand command)
            => await HandleApiResponseAsync<bool>(await httpClient.PutAsJsonAsync($"trips/{tripId}/segments/{segmentId}/destinations/{destinationId}", command));

        public async Task<Result<bool>> RemoveDestinationAsync(Guid tripId, Guid segmentId, Guid destinationId)
            => await HandleApiResponseAsync<bool>(await httpClient.DeleteAsync($"trips/{tripId}/segments/{segmentId}/destinations/{destinationId}"));
        #endregion

        #region Participants
        public async Task<Result<Guid>> AddParticipantAsync(Guid tripId, AddParticipantCommand command)
            => await HandleApiResponseAsync<Guid>(await httpClient.PostAsJsonAsync($"trips/{tripId}/participants", command));

        public async Task<Result<Guid>> AddGuestParticipantAsync(Guid tripId, AddGuestParticipantCommand command)
            => await HandleApiResponseAsync<Guid>(await httpClient.PostAsJsonAsync($"trips/{tripId}/participants/guest", command));

        public async Task<Result<bool>> UpdateParticipantAsync(Guid tripId, Guid participantId, UpdateParticipantCommand command)
            => await HandleApiResponseAsync<bool>(await httpClient.PutAsJsonAsync($"trips/{tripId}/participants/{participantId}", command));

        public async Task<Result<bool>> ChangeParticipantPermissionAsync(Guid tripId, Guid participantId, ChangeParticipantPermissionCommand command)
            => await HandleApiResponseAsync<bool>(await httpClient.PutAsJsonAsync($"trips/{tripId}/participants/{participantId}/permission", command));

        public async Task<Result<bool>> RemoveParticipantAsync(Guid tripId, Guid participantId)
            => await HandleApiResponseAsync<bool>(await httpClient.DeleteAsync($"trips/{tripId}/participants/{participantId}"));
        #endregion

        #region Transportation
        public async Task<Result<Guid>> AddTransportationAsync(Guid tripId, AddTransportationCommand command)
            => await HandleApiResponseAsync<Guid>(await httpClient.PostAsJsonAsync($"trips/{tripId}/transportations", command));

        public async Task<Result<bool>> UpdateTransportationAsync(Guid tripId, Guid transportationId, UpdateTransportationCommand command)
            => await HandleApiResponseAsync<bool>(await httpClient.PutAsJsonAsync($"trips/{tripId}/transportations/{transportationId}", command));

        public async Task<Result<bool>> RemoveTransportationAsync(Guid tripId, Guid transportationId)
            => await HandleApiResponseAsync<bool>(await httpClient.DeleteAsync($"trips/{tripId}/transportations/{transportationId}"));
        #endregion

        #region Accommodation
        public async Task<Result<Guid>> AddAccommodationAsync(Guid tripId, Guid destinationId, AddAccommodationCommand command)
            => await HandleApiResponseAsync<Guid>(await httpClient.PostAsJsonAsync($"trips/{tripId}/destinations/{destinationId}/accommodations", command));

        public async Task<Result<bool>> UpdateAccommodationAsync(Guid tripId, Guid destinationId, Guid accommodationId, UpdateAccommodationCommand command)
            => await HandleApiResponseAsync<bool>(await httpClient.PutAsJsonAsync($"trips/{tripId}/destinations/{destinationId}/accommodations/{accommodationId}", command));

        public async Task<Result<bool>> RemoveAccommodationAsync(Guid tripId, Guid destinationId, Guid accommodationId)
            => await HandleApiResponseAsync<bool>(await httpClient.DeleteAsync($"trips/{tripId}/destinations/{destinationId}/accommodations/{accommodationId}"));
        #endregion

        #region Activities
        public async Task<Result<Guid>> AddActivityAsync(Guid tripId, Guid destinationId, AddActivityCommand command)
            => await HandleApiResponseAsync<Guid>(await httpClient.PostAsJsonAsync($"trips/{tripId}/destinations/{destinationId}/activities", command));

        public async Task<Result<bool>> UpdateActivityAsync(Guid tripId, Guid destinationId, Guid activityId, UpdateActivityCommand command)
            => await HandleApiResponseAsync<bool>(await httpClient.PutAsJsonAsync($"trips/{tripId}/destinations/{destinationId}/activities/{activityId}", command));

        public async Task<Result<bool>> RemoveActivityAsync(Guid tripId, Guid destinationId, Guid activityId)
            => await HandleApiResponseAsync<bool>(await httpClient.DeleteAsync($"trips/{tripId}/destinations/{destinationId}/activities/{activityId}"));
        #endregion

        #region Error handling

        // Zentrale Fehlerbehandlung
        private static async Task<Result<T>> HandleApiResponseAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(bool))
                    return (Result<T>)(object)true;

                var value = await response.Content.ReadFromJsonAsync<T>();
                if (value is not null)
                    return value;
                return new ErrorDetail("NullResult", "Die Antwort der API war leer.");
            }
            var error = await TryReadErrorDetailAsync(response);
            return error ?? new ErrorDetail("Unknown", "Unbekannter Fehler");
        }

        private static async Task<ErrorDetail?> TryReadErrorDetailAsync(HttpResponseMessage response)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorDetail>();
                return error;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}

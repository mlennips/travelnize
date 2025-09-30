using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.Queries;
using System.Net.Http.Json;

namespace LIT.Travelnize.Services.Api
{
    public class TripsApiClient(HttpClient httpClient)
    {
        public async Task<Guid?> CreateTripAsync(CreateTripCommand command)
        {
            var response = await httpClient.PostAsJsonAsync("api/trips", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<ListTripsResponse[]?> GetTripsAsync(Guid userId)
            => await httpClient.GetFromJsonAsync<ListTripsResponse[]>($"api/trips?userId={userId}");

        public async Task<GetTripResponse?> GetTripAsync(Guid tripId)
            => await httpClient.GetFromJsonAsync<GetTripResponse>($"api/trips/{tripId}");

        public async Task<bool> UpdateTripAsync(Guid tripId, UpdateTripCommand command)
            => (await httpClient.PutAsJsonAsync($"api/trips/{tripId}", command)).IsSuccessStatusCode;

        public async Task<bool> DeleteTripAsync(Guid tripId)
            => (await httpClient.DeleteAsync($"api/trips/{tripId}")).IsSuccessStatusCode;

        public async Task<Guid?> AddTravelSegmentAsync(Guid tripId, AddTravelSegmentCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"api/trips/{tripId}/segments", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<bool> UpdateTravelSegmentAsync(Guid tripId, Guid segmentId, UpdateTravelSegmentCommand command)
            => (await httpClient.PutAsJsonAsync($"api/trips/{tripId}/segments/{segmentId}", command)).IsSuccessStatusCode;

        public async Task<bool> RemoveTravelSegmentAsync(Guid tripId, Guid segmentId)
            => (await httpClient.DeleteAsync($"api/trips/{tripId}/segments/{segmentId}")).IsSuccessStatusCode;

        public async Task<Guid?> AddDestinationAsync(Guid tripId, Guid segmentId, AddDestinationCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"api/trips/{tripId}/segments/{segmentId}/destinations", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<bool> UpdateDestinationAsync(Guid tripId, Guid segmentId, Guid destinationId, UpdateDestinationCommand command)
            => (await httpClient.PutAsJsonAsync($"api/trips/{tripId}/segments/{segmentId}/destinations/{destinationId}", command)).IsSuccessStatusCode;

        public async Task<bool> RemoveDestinationAsync(Guid tripId, Guid segmentId, Guid destinationId)
            => (await httpClient.DeleteAsync($"api/trips/{tripId}/segments/{segmentId}/destinations/{destinationId}")).IsSuccessStatusCode;

        public async Task<Guid?> AddParticipantAsync(Guid tripId, AddParticipantCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"api/trips/{tripId}/participants", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<Guid?> AddGuestParticipantAsync(Guid tripId, AddGuestParticipantCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"api/trips/{tripId}/participants/guest", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<bool> UpdateParticipantAsync(Guid tripId, Guid participantId, UpdateParticipantCommand command)
            => (await httpClient.PutAsJsonAsync($"api/trips/{tripId}/participants/{participantId}", command)).IsSuccessStatusCode;

        public async Task<bool> ChangeParticipantPermissionAsync(Guid tripId, Guid participantId, ChangeParticipantPermissionCommand command)
            => (await httpClient.PutAsJsonAsync($"api/trips/{tripId}/participants/{participantId}/permission", command)).IsSuccessStatusCode;

        public async Task<bool> RemoveParticipantAsync(Guid tripId, Guid participantId)
            => (await httpClient.DeleteAsync($"api/trips/{tripId}/participants/{participantId}")).IsSuccessStatusCode;

        public async Task<Guid?> AddTransportationAsync(Guid tripId, AddTransportationCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"api/trips/{tripId}/transportations", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<Guid?> AddAccommodationAsync(Guid tripId, Guid destinationId, AddAccommodationCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"api/trips/{tripId}/destinations/{destinationId}/accommodations", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<Guid?> AddActivityAsync(Guid tripId, Guid destinationId, AddActivityCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"api/trips/{tripId}/destinations/{destinationId}/activities", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<bool> UpdateActivityAsync(Guid tripId, Guid destinationId, Guid activityId, UpdateActivityCommand command)
            => (await httpClient.PutAsJsonAsync($"api/trips/{tripId}/destinations/{destinationId}/activities/{activityId}", command)).IsSuccessStatusCode;
    }
}

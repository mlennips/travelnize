using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.Queries;
using System.Net.Http.Json;

namespace LIT.Travelnize.Services.Api
{
    public class TripsApiClient(HttpClient httpClient)
    {
        // Trips
        public async Task<Guid?> CreateTripAsync(CreateTripCommand command)
        {
            var response = await httpClient.PostAsJsonAsync("/trips", command);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Guid>();
            }
            return null;
        }

        public async Task<ListTripsResponse[]?> GetTripsAsync(Guid userId)
        {
            return await httpClient.GetFromJsonAsync<ListTripsResponse[]>($"/trips?userId={userId}");
        }

        public async Task<GetTripResponse?> GetTripAsync(Guid tripId)
        {
            return await httpClient.GetFromJsonAsync<GetTripResponse>($"/trips/{tripId}");
        }

        public async Task<bool> UpdateTripAsync(Guid tripId, UpdateTripCommand command)
        {
            var response = await httpClient.PutAsJsonAsync($"/trips/{tripId}", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTripAsync(Guid tripId)
        {
            var response = await httpClient.DeleteAsync($"/trips/{tripId}");
            return response.IsSuccessStatusCode;
        }

        // Travel Segments
        public async Task<Guid?> AddTravelSegmentAsync(Guid tripId, AddTravelSegmentCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"/trips/{tripId}/segments", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<bool> UpdateTravelSegmentAsync(Guid tripId, Guid segmentId, UpdateTravelSegmentCommand command)
        {
            var response = await httpClient.PutAsJsonAsync($"/trips/{tripId}/segments/{segmentId}", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveTravelSegmentAsync(Guid tripId, Guid segmentId)
        {
            var response = await httpClient.DeleteAsync($"/trips/{tripId}/segments/{segmentId}");
            return response.IsSuccessStatusCode;
        }

        // Destinations
        public async Task<Guid?> AddDestinationAsync(Guid tripId, Guid segmentId, AddDestinationCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"/trips/{tripId}/segments/{segmentId}/destinations", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<bool> UpdateDestinationAsync(Guid tripId, Guid segmentId, Guid destinationId, UpdateDestinationCommand command)
        {
            var response = await httpClient.PutAsJsonAsync($"/trips/{tripId}/segments/{segmentId}/destinations/{destinationId}", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveDestinationAsync(Guid tripId, Guid segmentId, Guid destinationId)
        {
            var response = await httpClient.DeleteAsync($"/trips/{tripId}/segments/{segmentId}/destinations/{destinationId}");
            return response.IsSuccessStatusCode;
        }

        // Participants
        public async Task<Guid?> AddParticipantAsync(Guid tripId, AddParticipantCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"/trips/{tripId}/participants", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<Guid?> AddGuestParticipantAsync(Guid tripId, AddGuestParticipantCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"/trips/{tripId}/participants/guest", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<bool> UpdateParticipantAsync(Guid tripId, Guid participantId, UpdateParticipantCommand command)
        {
            var response = await httpClient.PutAsJsonAsync($"/trips/{tripId}/participants/{participantId}", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ChangeParticipantPermissionAsync(Guid tripId, Guid participantId, ChangeParticipantPermissionCommand command)
        {
            var response = await httpClient.PutAsJsonAsync($"/trips/{tripId}/participants/{participantId}/permission", command);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveParticipantAsync(Guid tripId, Guid participantId)
        {
            var response = await httpClient.DeleteAsync($"/trips/{tripId}/participants/{participantId}");
            return response.IsSuccessStatusCode;
        }

        // Transportation
        public async Task<Guid?> AddTransportationAsync(Guid tripId, AddTransportationCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"/trips/{tripId}/transportations", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        // Accommodation
        public async Task<Guid?> AddAccommodationAsync(Guid tripId, Guid destinationId, AddAccommodationCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"/trips/{tripId}/destinations/{destinationId}/accommodations", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        // Activities
        public async Task<Guid?> AddActivityAsync(Guid tripId, Guid destinationId, AddActivityCommand command)
        {
            var response = await httpClient.PostAsJsonAsync($"/trips/{tripId}/destinations/{destinationId}/activities", command);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Guid>();
            return null;
        }

        public async Task<bool> UpdateActivityAsync(Guid tripId, Guid destinationId, Guid activityId, UpdateActivityCommand command)
        {
            var response = await httpClient.PutAsJsonAsync($"/trips/{tripId}/destinations/{destinationId}/activities/{activityId}", command);
            return response.IsSuccessStatusCode;
        }
    }
}

using LIT.Travelnize.Domain.Trips.Commands;
using LIT.Travelnize.Domain.Trips.Queries;
using System.Net.Http.Json;

namespace LIT.Travelnize.Services.Api
{
    public class TripsApiClient(HttpClient httpClient)
    {
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
    }
}

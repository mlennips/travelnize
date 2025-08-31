using LIT.Travelnize.Shared.Trips;
using System.Net.Http.Json;

namespace LIT.Travelnize.Utils.Api
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

        public async Task<ListTripsDto[]?> GetTripsAsync(Guid userId)
        {
            return await httpClient.GetFromJsonAsync<ListTripsDto[]>($"/trips?userId={userId}");
        }

        public async Task<GetTripDto?> GetTripAsync(Guid tripId)
        {
            return await httpClient.GetFromJsonAsync<GetTripDto>($"/trips/{tripId}");
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

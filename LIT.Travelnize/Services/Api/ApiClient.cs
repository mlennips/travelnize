namespace LIT.Travelnize.Services.Api
{
    public class ApiClient(HttpClient httpClient)
    {
        public Uri? BaseAddress => httpClient.BaseAddress;

        public async Task<string> GetWelcomeAsync()
        {
            return await httpClient.GetStringAsync("api/welcome");
        }
    }
}

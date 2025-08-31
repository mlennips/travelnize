namespace LIT.Travelnize.Utils.Api
{
    public class ApiClient(HttpClient httpClient)
    {
        public async Task<string> GetWelcomeAsync()
        {
            return await httpClient.GetStringAsync("/welcome");
        }
    }
}

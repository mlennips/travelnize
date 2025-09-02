namespace LIT.Travelnize.Interfaces
{
    public interface IAccessTokenService
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task RemoveTokenAsync();
    }
}

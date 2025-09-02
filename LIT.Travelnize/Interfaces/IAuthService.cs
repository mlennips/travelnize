
namespace LIT.Travelnize.Interfaces
{
    public interface IAuthService
    {
        Task<IAuthUser?> GetCurrentUserAsync();
        Task<bool> LoginAsync(string emailOrUserName, string password);
        Task<bool> LogoutAsync();
        Task<bool> CheckIsAuthenticatedAsync();
    }
}

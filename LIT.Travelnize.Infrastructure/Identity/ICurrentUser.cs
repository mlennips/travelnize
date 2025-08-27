using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Infrastructure.Identity
{
    public interface ICurrentUser
    {
        Task<IUser> GetUserAsync();
    }
}

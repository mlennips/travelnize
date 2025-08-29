namespace LIT.Travelnize.Domain.Base
{
    public interface ICurrentUser
    {
        Task<IUser> GetUserAsync();
    }
}

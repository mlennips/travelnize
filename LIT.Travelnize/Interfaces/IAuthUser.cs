namespace LIT.Travelnize.Interfaces
{
    public interface IAuthUser
    {
        Guid UserId { get; }
        string UserName { get; }
        string FirstName { get; }
        string LastName { get; }
    }
}

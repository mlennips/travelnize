namespace LIT.Travelnize.Domain.Base
{
    public interface IUser
    {
        public Guid Id { get; }
        public string? UserName { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string? Email { get; }
    }
}

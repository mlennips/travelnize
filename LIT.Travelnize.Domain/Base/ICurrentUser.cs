namespace LIT.Travelnize.Domain.Base
{
    public interface ICurrentUser
    {
        Task<IUser> GetUserAsync();

        string Name { get; }
        string? RequestPath { get; }
        string? HttpMethod { get; }
        string? IpAddress { get; }
        string? UserAgent { get; }
        string? TraceIdentifier { get; }
        string? Referer { get; }
    }
}

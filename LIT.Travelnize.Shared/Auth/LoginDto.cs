namespace LIT.Travelnize.Shared.Auth
{
    public record LoginDto(string Token, Guid UserId, string UserName, string FirstName, string LastName)
    {
    }
}

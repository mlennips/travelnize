namespace LIT.Travelnize.Domain.Auth
{
    public record LoginDto(string Token, Guid UserId, string UserName, string FirstName, string LastName)
    {
    }
}

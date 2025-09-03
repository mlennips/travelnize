namespace LIT.Travelnize.Domain.Auth
{
    public record RefreshCommand
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = default!;
    }
}

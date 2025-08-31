namespace LIT.Travelnize.Shared.Auth
{
    public record RefreshCommand
    {
        public Guid UserId { get; set; }
        public string Token { get; set; } = default!;
    }
}

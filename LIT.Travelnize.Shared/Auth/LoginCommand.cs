namespace LIT.Travelnize.Shared.Auth
{
    public record LoginCommand
    {
        public string EmailOrUserName { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}

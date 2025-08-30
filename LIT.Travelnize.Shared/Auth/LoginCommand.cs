namespace LIT.Travelnize.Shared.Auth
{
    public record LoginCommand
    {
        public LoginCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; }
        public string Password { get; set; }
    }
}

namespace LIT.Travelnize.Shared.Auth
{
    public record RegisterCommand
    {
        public RegisterCommand(string email, string password, string confirmPassword)
        {
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}

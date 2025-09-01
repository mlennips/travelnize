using System.ComponentModel.DataAnnotations;

namespace LIT.Travelnize.Shared.Auth
{
    public record ForgotPasswordCommand
    {
        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}

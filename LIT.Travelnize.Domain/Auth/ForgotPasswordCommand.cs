using System.ComponentModel.DataAnnotations;

namespace LIT.Travelnize.Domain.Auth
{
    public record ForgotPasswordCommand
    {
        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}

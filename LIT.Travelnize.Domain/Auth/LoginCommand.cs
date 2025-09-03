using System.ComponentModel.DataAnnotations;

namespace LIT.Travelnize.Domain.Auth
{
    public record LoginCommand
    {
        [Required]
        public string EmailOrUserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

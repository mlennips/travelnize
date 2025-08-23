using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Common
{
    public record Email : SingleValueObject<string>
    {
        public Email(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value) || !IsValidEmail(value))
            {
                throw new ArgumentException("Invalid email address.");
            }
        }

        private static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith('.'))
            {
                return false; 
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
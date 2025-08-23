using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.Domain.Common
{
    public record ExternalUrl : SingleValueObject<string>
    {
        public ExternalUrl(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("URL cannot be empty.", nameof(value));

            if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                throw new ArgumentException("URL is not valid.", nameof(value));
        }
    }
}
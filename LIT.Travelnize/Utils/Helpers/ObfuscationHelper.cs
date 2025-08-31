using System.Security.Cryptography;
using System.Text;

namespace LIT.Travelnize.Utils.Helpers
{
    public static class ObfuscationHelper
    {
        private static readonly string DefaultKey = "tr4v3rn173";

        public static string Encrypt(string plainText, string? key = null)
        {
            key ??= DefaultKey;
            var output = new StringBuilder();
            for (int i = 0; i < plainText.Length; i++)
            {
                output.Append((char)(plainText[i] ^ key[i % key.Length]));
            }
            // Base64 für Speicherung als String
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(output.ToString()));
        }

        public static string Decrypt(string cipherText, string? key = null)
        {
            key ??= DefaultKey;
            var encrypted = Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));
            var output = new StringBuilder();
            for (int i = 0; i < encrypted.Length; i++)
            {
                output.Append((char)(encrypted[i] ^ key[i % key.Length]));
            }
            return output.ToString();
        }
    }
}

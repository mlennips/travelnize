using LIT.Travelnize.Helpers;
using Microsoft.JSInterop;
using System.Text.Json;

namespace LIT.Travelnize.Services
{
    public class LocalStorageService(IJSRuntime jsRuntime)
    {
        public async Task SetValueAsync<T>(string key, T value, bool obfuscate = false)
        {
            var json = JsonSerializer.Serialize(value);
            if (obfuscate)
            {
                // Optional: Obfuscate the json string before storing
                json = ObfuscationHelper.Encrypt(json);
            }
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }

        public async Task RemoveValueAsync(string key)
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task<T?> GetValueAsync<T>(string key, bool clarify = false) where T : class
        {
            var json = await jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            if (clarify)
            {
                // Optional: De-obfuscate the base64 string after retrieving
                json = ObfuscationHelper.Decrypt(json);
            }
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}

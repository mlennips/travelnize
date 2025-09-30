using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace LIT.Travelnize.Services.Api
{
    public class BackendBaseAddressResolver(WebAssemblyHostConfiguration Config, string HostEnvironmentBaseAddress, bool IsDev)
    {
        private Uri? baseAddress;

        public Uri BaseAddress
        {
            get 
            {
                if (baseAddress == null) 
                {
                    baseAddress = ResolveBackendBaseAddress();
                }
                return baseAddress;
            }
        }

        private Uri ResolveBackendBaseAddress()
        {
            var configured = Config["Backend:BaseUrl"]?.Trim();
            string EnsureSlash(string v) => v.EndsWith('/') ? v : v + "/";

            Uri MakeAbsolute(string value) => new(EnsureSlash(value), UriKind.Absolute);

            // Helper: Origin
            var origin = new Uri(HostEnvironmentBaseAddress);

            // 1. Konfig explizit?
            if (!string.IsNullOrWhiteSpace(configured))
            {
                // Absolute?
                if (Uri.TryCreate(configured, UriKind.Absolute, out var abs))
                {
                    // Schutz: interne / Docker-only Hosts (kein Punkt) in Produktion ignorieren
                    if (!IsDev && abs.Host.IndexOf('.') < 0)
                    {
                        var corrected = new Uri(origin, "api/");
                        Console.WriteLine($"[BackendBase] WARN ignoring internal host '{abs.Host}' in production -> {corrected}");
                        return corrected;
                    }

                    return MakeAbsolute(abs.AbsoluteUri);
                }

                // Relativ mit führendem /
                if (configured.StartsWith('/'))
                    return new Uri(origin, EnsureSlash(configured.TrimStart('/')));

                // Relativ ohne /
                return new Uri(origin, EnsureSlash(configured));
            }

            // 2. Fallbacks
            if (IsDev)
                return MakeAbsolute("https://localhost:5005/api/");

            // Produktion: gleiche Origin + /api/
            return new Uri(origin, "api/");
        }
    }
}

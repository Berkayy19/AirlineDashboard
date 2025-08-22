using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;            // wichtig fuer JsonDocument
using System.Net.Http;             // HttpClient
using Microsoft.Extensions.Configuration;

namespace FlightBackend.Services
{
    public class LufthansaAuthService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _cfg;
        private readonly IMemoryCache _cache;

        public LufthansaAuthService(HttpClient http, IConfiguration cfg, IMemoryCache cache)
        {
            _http = http;
            _cfg = cfg;
            _cache = cache;
        }

        public async Task<string> GetTokenAsync()
        {
            if (_cache.TryGetValue("lh.token", out string token))
                return token;

            var clientId = _cfg["Lufthansa:ClientId"];
            var clientSecret = _cfg["Lufthansa:ClientSecret"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                throw new InvalidOperationException("Lufthansa ClientId oder ClientSecret fehlt in der Konfiguration");

            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret
            });

            var res = await _http.PostAsync("https://api.lufthansa.com/v1/oauth/token", form);
            res.EnsureSuccessStatusCode();

            var json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var accessToken = root.GetProperty("access_token").GetString();
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new InvalidOperationException("access_token nicht in der Tokenantwort gefunden");

            int expiresIn = 3600;
            if (root.TryGetProperty("expires_in", out var expEl) && expEl.ValueKind == JsonValueKind.Number)
                expiresIn = expEl.GetInt32();

            // 5 Minuten Sicherheitsabzug
            var lifetime = TimeSpan.FromSeconds(Math.Max(300, expiresIn - 300));
            _cache.Set("lh.token", accessToken, lifetime);

            return accessToken!;
        }
    }
}

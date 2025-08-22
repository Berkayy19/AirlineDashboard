using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace FlightBackend.Services
{
    public class LufthansaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public LufthansaService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Lufthansa:ApiKey"] ?? string.Empty;
        }

        // Route-Schedules von A nach B am Datum
        public async Task<string> GetFlightsAsync(string from, string to, string date)
        {
            var url = $"https://api.lufthansa.com/v1/operations/schedules/{from}/{to}/{date}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // Flugstatus per Flugnummer am Datum, z B "LH400"
        public async Task<string> GetFlightStatusByNumberAsync(string flightNumber, string date)
        {
            var fn = flightNumber.Trim().ToUpperInvariant();
            var url = $"https://api.lufthansa.com/v1/operations/flightstatus/{fn}/{date}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
